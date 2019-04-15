using System;
using System.Threading.Tasks;
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

        bool IsClose { get; set; }

        ClientStatusEnum Status { get; set; }

        IWebSocketConnection Socket { get; set; }

        event Action<IClient, MsgEntity> EventMsgSended;

        void MsgSend(MsgEntity msg);

        Task MsgReceive(string msg);

        void Dispose();
    }
}
