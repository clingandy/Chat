﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ChatWeb.Config;
using Microsoft.Extensions.Options;

namespace ChatWeb.WebSocket
{
    public class ChannelManage : IChannelManage, IDisposable
    {
        public ConcurrentDictionary<string, ISubscriber> ChannelList { get; }

        public event Action<IChannelManage, ISubscriber> EventChannelAdded;
        public event Action<IChannelManage, ISubscriber> EventChannelRemoveed;

        public event Action<ISubscriber, IClient> EventClientAdded;
        public event Action<ISubscriber, IClient> EventClientRemoved;

        private readonly SemaphoreSlim _smp;  // 信号量，全部渠道限制多少个客户端连接并行发消息
        private readonly MessageConfigure _messageConfigure; // 消息配置
        

        public ChannelManage(IOptions<MessageConfigure> messageConfigure)
        {
            ChannelList = new ConcurrentDictionary<string, ISubscriber>();
            _messageConfigure = messageConfigure.Value;
            _smp = new SemaphoreSlim(_messageConfigure.TotalMaxDegreeOfParallelism);
        }

        public void ChannelClientAdd(IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var channel = client.Channel;

            ChannelList.AddOrUpdate(channel,
                s => AddSubscriberChannel(channel),
                (s, subscriber) =>
                {
                    subscriber.ClientAdd(client);
                    return subscriber;
                });
        }

        private ISubscriber AddSubscriberChannel(string channel)
        {
            var sub = new Subscriber(_messageConfigure, _smp, channel);
            ChannelList[channel] = sub;

            //添加Channel后的处理事件
            var handelerChannelAdded = EventChannelAdded;
            handelerChannelAdded?.Invoke(this, sub);

            sub.EventClientAdded += (subscriber, client1) =>
            {
                var handeler = EventClientAdded;
                handeler?.Invoke(subscriber, client1);
            };
            sub.EventClientRemoved += (subscriber, client1) =>
            {
                if (subscriber.DicClientSockets.IsEmpty)
                {
                    ChannelList.Remove(subscriber.ChannelName, out _);
                    //移除Channel后的处理事件
                    var handelerChannelRemoveed = EventChannelRemoveed;
                    handelerChannelRemoveed?.Invoke(this, subscriber);

                    subscriber.Dispose();
                }

                var handeler = EventClientRemoved;
                handeler?.Invoke(subscriber, client1);
            };

            return sub;
        }

        public void ChannelClientRemove(string channel, string clientId)
        {
            if (ChannelList.ContainsKey(channel))
            {
                ChannelList[channel].ClientRemove(clientId);
            }
        }

        public void Dispose()
        {
            EventChannelAdded = null;
            EventChannelRemoveed = null;
            EventClientAdded = null;
            EventClientRemoved = null;
        }
    }
}
