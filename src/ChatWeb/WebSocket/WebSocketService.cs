using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Fleck;
using ChatWeb.Model;
using ChatWeb.Redis;
using ChatWeb.Tool;

namespace ChatWeb.WebSocket
{
    public class WebSocketService: IDisposable
    {
        private readonly RedisMessageManage _redisMessageManage;

        private static ConcurrentDictionary<string, List<IWebSocketConnection>> _dicSockets;

        private ActionBlock<SendMsgQueueBatchEntity> _sendMsgActionBlockBatch;

        public WebSocketService(RedisMessageManage redis)
        {
            _redisMessageManage = redis;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitWebSocker()
        {
            _dicSockets = new ConcurrentDictionary<string, List<IWebSocketConnection>>();

            _sendMsgActionBlockBatch = new ActionBlock<SendMsgQueueBatchEntity>(model =>
            {
                Parallel.ForEach(model.SocketList, (item) =>
                {
                    item.Send(model.Msg);
                });
            });

            var server = new WebSocketServer("ws://0.0.0.0:7091");
            server.RestartAfterListenError = true;
            //不要使用子协议，使用后明显变慢

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    ConnOpen(socket);
                };

                socket.OnClose = () =>
                {
                    LoginOutAndClose(socket);
                };

                socket.OnError = (e) =>
                {
                    LoginOutAndClose(socket); //网络断开
                };

                socket.OnMessage = message =>
                {
                    var parameter = socket.ConnectionInfo.Path.Replace("/?", "").Split("?");
                    var channel = parameter[0];
                    var userId = parameter[1];
                    _redisMessageManage.SendMsg(channel, message);
                };

                socket.OnBinary = message =>
                {
                    
                };
            });

        }

        /// <summary>
        /// 客户端连接后回调
        /// </summary>
        /// <param name="socket"></param>
        private void ConnOpen(IWebSocketConnection socket)
        {
            var parameter = socket.ConnectionInfo.Path.Replace("/?", "").Split("?");
            var channel = parameter[0];
            var userId = parameter[1];

            if (!_dicSockets.ContainsKey(channel))
            {
                if (!_redisMessageManage.CheckChannel(channel))
                {
                    socket.Close(); //关闭，效率比子协议好
                    return;
                }
                _dicSockets[channel] = new List<IWebSocketConnection> {socket};
                Task.Factory.StartNew(() =>
                {
                    _redisMessageManage.OnSubscribe(channel, msg =>
                    {
                        if (!_dicSockets.ContainsKey(channel))
                        {
                            return;
                        }
                        try
                        {
                            _sendMsgActionBlockBatch.Post(new SendMsgQueueBatchEntity{ SocketList = _dicSockets[channel], Msg = msg });
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    });
                });
            }
            else
            {
                _dicSockets[channel].Add(socket);
            }

            //登录消息
            PubLoginMsg(channel, userId, true);
        }

        /// <summary>
        /// 连接关闭和异常回调
        /// </summary>
        /// <param name="socket"></param>
        private void LoginOutAndClose(IWebSocketConnection socket)
        {
            var parameter = socket.ConnectionInfo.Path.Replace("/?", "").Split("?");
            var channel = parameter[0];
            var userId = parameter[1];
            //登出消息
            PubLoginMsg(channel, userId, false);
            //删除连接
            _dicSockets[channel].Remove(socket);
        }

        /// <summary>
        /// 发布Redis登录登出消息
        /// </summary>
        private void PubLoginMsg(string channel, string userId, bool islogin)
        {
            Task.Factory.StartNew(() =>
            {
                if (islogin)
                {
                    _redisMessageManage.AddChannelSubscribeUser(channel, userId);
                }
                else
                {
                    _redisMessageManage.DelChannelSubscribeUser(channel, userId);
                }

                //测试用
                var msg = new MsgEntity
                {
                    Type = islogin ? (int)MsgTypeEnum.登录 : (int)MsgTypeEnum.登出,
                    Data = _dicSockets[channel].Count.ToString(),
                    Code = 200,
                    UserId = userId
                }.JsonSerialize();

                _redisMessageManage.SendMsg(channel, msg);
            });
        }

        public void Dispose()
        {

        }
    }
}