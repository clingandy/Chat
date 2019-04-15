using System;
using Fleck;
using ChatWeb.Model;
using ChatWeb.Redis;
using ChatWeb.Tool;
using System.Linq;
using System.Threading.Tasks;
using ChatWeb.Enum;

namespace ChatWeb.WebSocket
{
    public class ChatService: IDisposable
    {
        private readonly RedisMessageManage _redisMessageManage;

        private readonly IChannelManage _channelManage;

        public ChatService(RedisMessageManage redis, IChannelManage channelManage)
        {
            _redisMessageManage = redis;

            _channelManage = channelManage;
        }

        /// <summary>
        /// 初始化
        /// <para>聊天室、个人群聊：两者应该分开，为方便这里就写一起</para>
        /// </summary>
        public void InitWebSocker()
        {
            InitChannelEventMethod();

            var server = new WebSocketServer("ws://0.0.0.0:7091");
            server.RestartAfterListenError = true;
            server.ListenerSocket.NoDelay = true;

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
            });
        }

        /// <summary>
        /// 客户端连接后回调
        /// </summary>
        /// <param name="socket"></param>
        private void ConnOpen(IWebSocketConnection socket)
        {
            var client = new Client(socket);

            _channelManage.ChannelClientAdd(client);
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
            //删除连接
            _channelManage.ChannelClientRemove(channel, userId);
        }

        /// <summary>
        /// 初始化事件
        /// <para>想法，消息可以合并在一起一起发送</para>
        /// <para>***本设计主要还是属于聊天室的</para>
        /// <para>***聊天室、个人（群聊和个人类似） 这两者应该分开处理，这里简单统一处理</para>
        /// </summary>
        private void InitChannelEventMethod()
        {
            // 添加渠道后
            _channelManage.EventChannelAdded += (manage, subscriber) =>
            {
                _redisMessageManage.OnSubscribe(subscriber.ChannelName, msg =>
                {
                    var model = msg.JsonDeserialize<MsgEntity>();
                    subscriber.NotifyAllClient(model);
                });             
            };
            // 移除渠道后
            _channelManage.EventChannelRemoveed += (manage, subscriber) =>
            {
                _redisMessageManage.OnUnSubscribe(subscriber.ChannelName);
            };

            // 添加渠道用户后
            _channelManage.EventClientAdded += (subscriber, client) =>
            {
                //client.EventMsgSended += subscriber.NotifyAllClient;    //聊天室单机非Redis模式，接受到消息后直接转发

                client.EventMsgSended += (that ,msgEntity) =>
                {
                    if (that.Status != ClientStatusEnum.OnLine || msgEntity.Type == (int)MsgTypeEnum.禁止发送)
                    {
                        SystemNotify(that, "你已经被禁言 或 发言过于频繁");
                        return;
                    }
                    // 判断是个人群聊还是 聊天室
                    if (client.Channel != client.ClientId)
                    {
                        _redisMessageManage.SendMsg(client.Channel, msgEntity);
                    }
                    else
                    {
                        switch (msgEntity.Type)
                        {
                            case (int)MsgTypeEnum.文本:
                                // 用户不在线保存数据， 这里默认保存
                                _redisMessageManage.SendMsg(msgEntity.ToId, msgEntity, true);
                                break;
                            case (int)MsgTypeEnum.请求添加好友:
                                _redisMessageManage.AddUser(new UserEntity { Status = (int)ClientStatusEnum.OnLine, UserId = msgEntity.ToId, UserName = msgEntity.Data });
                                break;
                        }
                    }
                    
                };

                // LoginNotify(subscriber, client);  // 发送登录消息

                if (client.ClientId != client.Channel)
                {
                    // _redisMessageManage.AddChannelSubscribeUser(client.Channel, client.ClientId);   // 添加在线用户
                }
                else
                {
                    GetUserAndMsgList(client);    //获取好友、历史消息
                }

                // Console.WriteLine($"【{subscriber.ChannelName}】上线数量：{subscriber.DicClientSockets.Count}");
            };
            // 移除渠道用户后
            _channelManage.EventClientRemoved += (subscriber, client) =>
            {
                // LoginNotify(subscriber, client); // 发送登出消息

                if (client.ClientId != client.Channel)
                {
                    // _redisMessageManage.DelChannelSubscribeUser(client.Channel, client.ClientId);   // 移除在线用户
                }
                else
                {
                    _redisMessageManage.DelAllMsg(client.ClientId);
                }
            };
        }

        /// <summary>
        /// 发送登录登出信息
        /// </summary>
        /// <param name="iSubscriber"></param>
        /// <param name="client"></param>
        private void LoginNotify(ISubscriber iSubscriber, IClient client)
        {
            Task.Factory.StartNew(() =>
            {
                var msgModel = new MsgEntity
                {
                    MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                    Type = client.Status == ClientStatusEnum.SignOut ? (int)MsgTypeEnum.登出 : (int)MsgTypeEnum.登录,
                    Data = iSubscriber.DicClientSockets.Count.ToString(),
                    CurTime = DateTime.Now.ConvertDateTimeToInt(),
                    FromId = client.ClientId,
                    FromName = client.ClientName
                };
                _redisMessageManage.SendMsg(client.Channel, msgModel);

                //iSubscriber.NotifyAllClient(msgModel); // 聊天室单机非Redis模式
            });
        }

        /// <summary>
        /// 发送系统信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        private void SystemNotify(IClient client, string msg)
        {
            var msgModel = new MsgEntity
            {
                MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                Type = (int)MsgTypeEnum.系统,
                Data = msg,
                CurTime = DateTime.Now.ConvertDateTimeToInt(),
                FromId = client.ClientId,
                FromName = client.ClientName
            };
            client.Socket?.Send(new []{ msgModel }.JsonSerialize());
        }

        /// <summary>
        /// 好友数据、历史消息
        /// </summary>
        /// <param name="client"></param>
        private async void GetUserAndMsgList(IClient client)
        {
            var userListStr = await _redisMessageManage.GetUserList();
            var userListMsg = new MsgEntity
            {
                MsgId = Guid.NewGuid().ToString().Replace("-", "").ToLower(),
                Type = (int)MsgTypeEnum.获取好友数据,
                Data = userListStr.JsonSerialize(),
                CurTime = DateTime.Now.ConvertDateTimeToInt(),
                FromId = client.ClientId,
                ToId = string.Empty
            };
            client.Socket?.Send(new []{ userListMsg }.JsonSerialize());


            var msgList = await _redisMessageManage.GetMsgList(client.ClientId);
            msgList = msgList.OrderBy(t => t.CurTime).ToList();
            foreach (var msg in msgList)
            {
                client.Socket?.Send(new[] { msg }.JsonSerialize());
            }
            await _redisMessageManage.DelAllMsg(client.ClientId);

            //while (true)
            //{
            //    Thread.Sleep(5);
            //    var msg = await _redisMessageManage.GetNextMsg(client.ClientId);
            //    if (msg == null || client.Socket == null) break;
            //    client.Socket?.Send(msg.JsonSerialize());
            //}
        }

        public void Dispose()
        {

        }
    }
}