using System;
using System.Collections.Concurrent;

namespace ChatWeb.WebSocket
{
    public interface IChannelManage
    {
        ConcurrentDictionary<string ,ISubscriber> ChannelList { get; }

        event Action<IChannelManage, ISubscriber> EventChannelAdded;
        event Action<IChannelManage, ISubscriber> EventChannelRemoveed;

        event Action<ISubscriber, IClient> EventClientAdded;
        event Action<ISubscriber, IClient> EventClientRemoved;

        void ChannelClientAdd(IClient client);

        void ChannelClientRemove(string channel, string clientId);
    }
}
