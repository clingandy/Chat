using System;
using System.Collections.Concurrent;
using ChatWeb.Model;

namespace ChatWeb.WebSocket
{
    public interface ISubscriber
    {
        string ChannelName { get; }

        ConcurrentDictionary<string, IClient> DicClientSockets { get;}

        event Action<ISubscriber, IClient> EventClientAdded;

        event Action<ISubscriber, IClient> EventClientRemoved;

        void NotifyAllClient(MsgEntity model);

        void ClientAdd(IClient client);

        void ClientRemove(string clientId);

        void Dispose();
    }
}
