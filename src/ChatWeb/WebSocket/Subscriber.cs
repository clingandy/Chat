using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ChatWeb.Config;
using ChatWeb.Enum;
using ChatWeb.Model;
using ChatWeb.Tool;

namespace ChatWeb.WebSocket
{
    public class Subscriber : ISubscriber, IDisposable
    {
        private int _count;

        //防止并发
        private readonly ActionBlock<IClient> _addRemoveBlock;
        private readonly BatchBlock<MsgEntity> _sendMsgBatchBlock;
        private const int ProcessingMsg = 1;    //正在发送处理消息数据
        private const int UnProcessingMsg = 0;  //没有发送处理消息数据
        private int _isProcessingMsg;  //是否正在发送处理数据

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
                ClientPost(client);
            });

            var spanTime = AppSettingsHelper.GetInt32("SendMsgSpanTime", 5);
            var sendMsgActionBlock = new ActionBlock<MsgEntity[]>(entity =>
            {
                //限制同时发送消息数量，限制带宽
                AppConfigure.Smp.Wait();

                //发送
                var msg = entity.JsonSerialize();
                Parallel.ForEach(DicClientSockets, client =>
                {
                    client.Value.MsgReceive(msg);
                });

                AppConfigure.Smp.Release();
                SpinWait.SpinUntil(() => _count < 100, _count / 100 * spanTime);
            });

            // 队列超过5K ， 不在入队，合并5条后发送
            _sendMsgBatchBlock = new BatchBlock<MsgEntity>(5, new GroupingDataflowBlockOptions { Greedy = true, BoundedCapacity = 5000 });
            _sendMsgBatchBlock.LinkTo(sendMsgActionBlock);
        }

        private void ClientPost(IClient client)
        {
            if (client.Status == ClientStatusEnum.SignOut)
            {
                if (DicClientSockets.Remove(client.ClientId, out IClient temp))
                {
                    Interlocked.Add(ref _count, -1);

                    client = temp;  //补充完整信息
                    client.Status = ClientStatusEnum.SignOut;

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
                    if (tempClient != null)
                    {
                        if (tempClient.Socket != null)
                        {
                            tempClient.Socket.OnClose = () => { };  //移除委托
                            tempClient.Socket.OnError = e => { };  //移除委托
                            // 可在这里发送消息通知其他地方已经登录
                            tempClient.Socket.Close();
                        }
                        tempClient.Socket = client.Socket;
                    }
                }
                else
                {
                    Interlocked.Add(ref _count, 1);
                    DicClientSockets[client.ClientId] = client;
                }

                var handler = EventClientAdded;
                handler?.Invoke(this, client);
            }           
        }

        public void NotifyAllClient(MsgEntity model)
        {
            _sendMsgBatchBlock.Post(model);
            TriggerBatchMsg();
        }

        /// <summary>
        /// N秒后处理BatchBlock对象没有发送的数据
        /// </summary>
        public void TriggerBatchMsg()
        {
            if (Interlocked.CompareExchange(ref _isProcessingMsg, ProcessingMsg, UnProcessingMsg) == UnProcessingMsg)
            {
                Task.Factory.StartNew(() =>
                {
                    SpinWait.SpinUntil(() => false, 1000);
                    _sendMsgBatchBlock.TriggerBatch();
                    Interlocked.Exchange(ref _isProcessingMsg, UnProcessingMsg);
                });
            }
        }

        public void ClientAdd(IClient client)
        {
            _addRemoveBlock.Post(client);
        }

        public void ClientRemove(string clientId)
        {
            var client = new Client(clientId, ClientStatusEnum.SignOut);
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
