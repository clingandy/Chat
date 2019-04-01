using System;
using ChatWeb.Enum;
using ChatWeb.Model;
using Fleck;

namespace ChatWeb.WebSocket
{                                                                 
    public interface IClient
    {
        string ClientId { get; }

        string ClientName { get; }

        string Channel { get; }

        ClientStatusEnum Status { get; set; }

        IWebSocketConnection Socket { get; set; }

        event Action<IClient, MsgEntity> EventMsgSended;

        //event Action<IClient, MsgEntity> EvenReceiveMsg;

        void MsgSend(MsgEntity msg);

        void MsgReceive(string msg);

        void Dispose();
    }
}
