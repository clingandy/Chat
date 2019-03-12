using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Fleck;
using PubSubWeb.Model;
using PubSubWeb.Redis;
using PubSubWeb.Tool;

namespace PubSubWeb.WebSocket
{
    public class WebSocket
    {
        private static int _maxDegreeOfParallelism = 50;
        private static ConcurrentDictionary<string, List<IWebSocketConnection>> _dicSockets;


        /// <summary>
        /// 并发发送消息 方案一
        /// </summary>
        private readonly ActionBlock<SendMsgQueueSingleEntity> _sendMsgActionBlockSingle = new ActionBlock<SendMsgQueueSingleEntity>(model =>
            {
                model.Socket.Send(model.Msg);
            }
            , new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism });

        /// <summary>
        /// 并发发送消息 方案二
        /// </summary>
        //private readonly ActionBlock<SendMsgQueueBatchEntity> _sendMsgActionBlockBatch = new ActionBlock<SendMsgQueueBatchEntity>(model =>
        //    {
        //        foreach (var socket in model.SocketList)
        //        {
        //            socket.Send(model.Msg);
        //        }
        //    });

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitWebSocker()
        {
            _maxDegreeOfParallelism = AppSettingsHelper.GetInt32("MaxDegreeOfParallelism");
            _dicSockets = new ConcurrentDictionary<string, List<IWebSocketConnection>>();
            
            var server = new WebSocketServer("ws://0.0.0.0:7091");
            server.RestartAfterListenError = true;
            //SubProtocolHandle(server);    //设置后明显变慢

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

                };
            });

        }

        /// <summary>
        /// 子协议
        /// </summary>
        /// <param name="server"></param>
        private void SubProtocolHandle(WebSocketServer server)
        {
            var supportedSubProtocols = RedisHelper.Instance.GetChannelList() ?? new List<string>();
            server.SupportedSubProtocols = supportedSubProtocols;
            Task.Factory.StartNew(() =>
            {
                RedisHelper.Instance.OnSubscribe("add_channel", channel =>
                {
                    supportedSubProtocols.Add(channel);
                    server.SupportedSubProtocols = supportedSubProtocols;
                });
            });
            Task.Factory.StartNew(() =>
            {
                RedisHelper.Instance.OnSubscribe("del_channel", channel =>
                {
                    supportedSubProtocols.Remove(channel);
                    server.SupportedSubProtocols = supportedSubProtocols;
                });
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
                if (!RedisHelper.Instance.CheckChannel(channel))
                {
                    socket.Close(); //关闭，效率比子协议好
                    return;
                }
                _dicSockets[channel] = new List<IWebSocketConnection> {socket};
                Task.Factory.StartNew(() =>
                {
                    RedisHelper.Instance.OnSubscribe(channel, msg =>
                    {
                        if (!_dicSockets.ContainsKey(channel))
                        {
                            return;
                        }
                        try
                        {
                            // 方案一
                            _dicSockets[channel].ForEach(item =>
                            {
                                _sendMsgActionBlockSingle.Post(new SendMsgQueueSingleEntity { Socket = item, Msg = msg });
                            });

                            //方案二
                            //_sendMsgActionBlockBatch.Post(new SendMsgQueueBatchEntity{ SocketList = _dicSockets[channel], Msg = msg });
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
                    RedisHelper.Instance.AddChannelSubscribeUser(channel, userId);
                }
                else
                {
                    RedisHelper.Instance.DelChannelSubscribeUser(channel, userId);
                }

                //测试用
                var msg = new MsgEntity
                {
                    Type = islogin ? (int)MsgTypeEnum.登录 : (int)MsgTypeEnum.登出,
                    Data = _dicSockets[channel].Count.ToString(),
                    Code = 200,
                    UserId = userId
                }.JsonSerialize();

                RedisHelper.Instance.SendMsg(channel, msg);
            });
        }

    }
}