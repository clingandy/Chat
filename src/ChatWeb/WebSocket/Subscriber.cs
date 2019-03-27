using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ChatWeb.Model;
using ChatWeb.Tool;

namespace ChatWeb.WebSocket
{
    public class Subscriber : ISubscriber, IDisposable
    {
        //防止并发
        private readonly ActionBlock<IClient> _addRemoveBlock;
        private readonly ActionBlock<MsgEntity> _sendMsgActionBlockBatch;

        public string ChannelName { get;}

        public bool IsEmpty { get; private set; }

        public ConcurrentDictionary<string, IClient> DicClientSockets { get;}

        public event Action<ISubscriber, IClient> EventClientAdded;
        public event Action<ISubscriber, IClient> EventClientRemoved;

        public Subscriber(string channelName)
        {
            ChannelName = channelName;

            DicClientSockets = new ConcurrentDictionary<string, IClient>();

            _addRemoveBlock = new ActionBlock<IClient>(client =>
            {
                //限制1秒上下线用户数量，内存换带宽
                Thread.Sleep(50);

                ClientPost(client);
            });

            _sendMsgActionBlockBatch = new ActionBlock<MsgEntity>(entity =>
            {
                //限制1秒发送消息数量，内存换带宽
                Thread.Sleep(50);

                var msg = entity.JsonSerialize();
                Parallel.ForEach(DicClientSockets, client =>
                {
                    client.Value.MsgReceive(msg);
                });
            });
        }

        private void ClientPost(IClient client)
        {
            if (client.IsSignOut)
            {
                if (DicClientSockets.Remove(client.ClientId, out IClient temp))
                {
                    client = temp;  //补充完整信息
                    client.IsSignOut = true;
                }

                IsEmpty = DicClientSockets.IsEmpty;

                var handler = EventClientRemoved;
                handler?.Invoke(this, client);

                client.Dispose();
            }
            else
            {
                // 非聊天室通知下线
                if (client.ClientId == client.Channel && DicClientSockets.ContainsKey(client.ClientId))
                {
                    var tempClient = DicClientSockets[client.ClientId];
                    tempClient?.Socket?.Close();
                }

                //聊天室不用考虑是否已经存在
                DicClientSockets[client.ClientId] = client;

                var handler = EventClientAdded;
                handler?.Invoke(this, client);
            }           
        }

        public void NotifyAllClient(MsgEntity model)
        {
            Task.Factory.StartNew(() =>
            {
                _sendMsgActionBlockBatch.Post(model);
            });
        }

        public void ClientAdd(IClient client)
        {
            Task.Factory.StartNew(() =>
            {
                client.IsSignOut = false;
                _addRemoveBlock.Post(client);
            });
        }

        public void ClientRemove(string clientId)
        {
            Task.Factory.StartNew(() =>
            {
                var client = new Client(clientId, true);
                _addRemoveBlock.Post(client);
            });
        }

        public bool CheckClientIsEmpty()
        {
            return DicClientSockets.IsEmpty;
        }

        public int GetClientCount()
        {
            return DicClientSockets.Count;
        }

        public void Dispose()
        {
            //_addRemoveBlock = null;
            //_sendMsgActionBlockBatch = null;
            //DicClientSockets = null;
            EventClientAdded = null;
            EventClientRemoved = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }

    }

}
