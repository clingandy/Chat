using System;
using ChatWeb.Model;
using Fleck;

namespace ChatWeb.WebSocket
{                                                                 
    public interface IClient
    {
        string ClientId { get; }

        string ClientName { get; }

        string Channel { get; }

        bool IsSignOut { get; set; }

        IWebSocketConnection Socket { get; }


        event Action<MsgEntity> EventMsgSended;

        //event Action<IClient, MsgEntity> EvenReceiveMsg;

        void MsgSend(MsgEntity msg);

        void MsgReceive(string msg);

        void Dispose();
    }
}
