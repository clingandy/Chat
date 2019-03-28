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
        private int _count;
        private int _spanTime;

        //防止并发
        private readonly ActionBlock<IClient> _addRemoveBlock;
        //private readonly BatchBlock<MsgEntity> _sendMsgBatchBlock;
        private readonly ActionBlock<MsgEntity> _sendMsgActionBlock;

        public string ChannelName { get;}

        public bool IsEmpty { get; private set; }

        public ConcurrentDictionary<string, IClient> DicClientSockets { get;}

        public event Action<ISubscriber, IClient> EventClientAdded;
        public event Action<ISubscriber, IClient> EventClientRemoved;

        public Subscriber(string channelName)
        {
            ChannelName = channelName;

            _spanTime = AppSettingsHelper.GetInt32("SendMsgSpanTime", 5);

            DicClientSockets = new ConcurrentDictionary<string, IClient>();

            _addRemoveBlock = new ActionBlock<IClient>(client =>
            {
                ClientPost(client);
            });

            // 队列超过5K ， 不在入队
            //_sendMsgBatchBlock = new BatchBlock<MsgEntity>(1, new GroupingDataflowBlockOptions{Greedy = true, BoundedCapacity = 5000});
            //_sendMsgBatchBlock.LinkTo(_sendMsgActionBlock);

            _sendMsgActionBlock = new ActionBlock<MsgEntity>(entity =>
            {
                //限制同时发送消息数量，限制带宽
                Configure.Smp.Wait();

                //发送
                var msg = entity.JsonSerialize();
                Parallel.ForEach(DicClientSockets, client =>
                {
                    client.Value.MsgReceive(msg);
                });

                Configure.Smp.Release();
                SpinWait.SpinUntil(() => _count < 100, _count / 100 * _spanTime);
            });
        }

        private void ClientPost(IClient client)
        {
            if (client.IsSignOut)
            {
                if (DicClientSockets.Remove(client.ClientId, out IClient temp))
                {
                    Interlocked.Add(ref _count, -1);

                    client = temp;  //补充完整信息
                    client.IsSignOut = true;

                    var handler = EventClientRemoved;
                    handler?.Invoke(this, client);
                }

                //IsEmpty = DicClientSockets.IsEmpty;
                IsEmpty = _count <= 0;

                client.Dispose();
            }
            else
            {
                if (DicClientSockets.ContainsKey(client.ClientId))
                {
                    var tempClient = DicClientSockets[client.ClientId];
                    if (tempClient?.Socket != null)
                    {
                        tempClient.Socket.OnClose = () => { };  //移除委托
                        tempClient.Socket.OnError = e => { };  //移除委托
                        // 可在这里发送占用消息
                        tempClient.Socket.Close();
                    }
                }
                else
                {
                    Interlocked.Add(ref _count, 1);
                }

                DicClientSockets[client.ClientId] = client;

                var handler = EventClientAdded;
                handler?.Invoke(this, client);
            }           
        }

        public void NotifyAllClient(MsgEntity model)
        {
            _sendMsgActionBlock.Post(model);
        }

        public void ClientAdd(IClient client)
        {
            client.IsSignOut = false;
            _addRemoveBlock.Post(client);
        }

        public void ClientRemove(string clientId)
        {
            var client = new Client(clientId, true);
            _addRemoveBlock.Post(client);
        }

        public bool CheckClientIsEmpty()
        {
            //return DicClientSockets.IsEmpty;
            return _count <= 0;
        }

        public int GetClientCount()
        {
            //return DicClientSockets.Count;
            return _count;
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
