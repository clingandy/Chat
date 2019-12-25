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
        //防止并发
        private readonly ActionBlock<IClient> _addRemoveBlock;
        private readonly BatchBlock<MsgEntity> _sendMsgBatchBlock;
        private const int ProcessingMsg = 1;    //正在发送处理消息数据
        private const int UnProcessingMsg = 0;  //没有发送处理消息数据
        private int _isProcessingMsg;  //是否正在发送处理数据
        private readonly SemaphoreSlim _smp;  // 信号量，全部渠道限制多少个客户端连接并行发消息

        public string ChannelName { get;}

        public ConcurrentDictionary<string, IClient> DicClientSockets { get;}

        public event Action<ISubscriber, IClient> EventClientAdded;
        public event Action<ISubscriber, IClient> EventClientRemoved;

        public Subscriber(MessageConfigure messageConfigure , SemaphoreSlim smp, string channelName)
        {
            ChannelName = channelName;

            DicClientSockets = new ConcurrentDictionary<string, IClient>();

            _addRemoveBlock = new ActionBlock<IClient>(client =>
            {
                ClientPost(client);
            });
            var sendMsgActionBlock = new ActionBlock<MsgEntity[]>(entity =>
            {
                var msg = entity.JsonSerialize();

                //限制同时发送消息数量，限制带宽；队列限制1W条
                //具体配置按带宽调整
                smp.Wait();
                //Parallel.ForEach(DicClientSockets, new ParallelOptions { MaxDegreeOfParallelism = messageConfigure.ChannelMaxDegreeOfParallelism }, item =>
                Parallel.ForEach(DicClientSockets, item =>
                {
                    item.Value.MsgReceive(msg);
                    Thread.Sleep(messageConfigure.SendMsgSpanTime);
                });
                smp.Release();
            }, new ExecutionDataflowBlockOptions { BoundedCapacity = messageConfigure.BoundedCapacity });

            // 合并5条后发送
            _sendMsgBatchBlock = new BatchBlock<MsgEntity>(5);
            _sendMsgBatchBlock.LinkTo(sendMsgActionBlock);
        }

        private void ClientPost(IClient client)
        {
            if (client.Status == ClientStatusEnum.SignOut)
            {
                if (DicClientSockets.Remove(client.ClientId, out IClient temp))
                {
                    client = temp;  //补充完整信息
                    client.Status = ClientStatusEnum.SignOut;

                    var handler = EventClientRemoved;
                    handler?.Invoke(this, client);
                }

                client.Dispose();
            }
            else
            {
                DicClientSockets.AddOrUpdate(client.ClientId, s =>
                {
                    var handler = EventClientAdded;
                    handler?.Invoke(this, client);
                    return client;
                }, (s, tempClient) =>
                {
                    if (tempClient.Socket != null)
                    {
                        tempClient.Socket.OnClose = () => { };  //移除委托
                        tempClient.Socket.OnError = e => { };  //移除委托
                        // 可在这里发送消息通知其他地方已经登录
                        tempClient.Socket.Close();
                    }
                    tempClient.Socket = client.Socket;
                    return client;
                });
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

        public void Dispose()
        {
            EventClientAdded = null;
            EventClientRemoved = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

    }

}
