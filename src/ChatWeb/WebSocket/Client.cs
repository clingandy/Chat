using System;
using System.Text;
using System.Web;
using ChatWeb.Model;
using ChatWeb.Tool;
using Fleck;

namespace ChatWeb.WebSocket
{
    public class Client : IClient, IDisposable
    {
        public string ClientId { get; }

        public string ClientName { get; }

        public string Channel { get; }

        public bool IsSignOut { get; set; }

        public IWebSocketConnection Socket { get; }

        public event Action<MsgEntity> EventMsgSended;

        public Client(IWebSocketConnection socket)
        {
            if (socket == null)
            {
                return;
            }
            var parameter = socket.ConnectionInfo.Path.Replace("/?", "").Split("?");
            Channel = parameter[0];
            ClientId = parameter[1];
            ClientName = HttpUtility.UrlDecode(parameter[2], Encoding.UTF8);

             Socket = socket;

            socket.OnMessage = message =>
            {
                MsgSend(message.JsonDeserialize<MsgEntity>());
            };
        }

        public Client(string clientId, bool isSignOut)
        {
            ClientId = clientId;
            IsSignOut = isSignOut;
        }

        public void MsgSend(MsgEntity msg)
        {
            msg.FromId = ClientId;
            msg.FromName = ClientName;
            msg.CurTime = DateTime.Now.ConvertDateTimeToInt();
            var handler = EventMsgSended;
            handler?.Invoke(msg);
        }

        public void MsgReceive(string msg)
        {
            Socket?.Send(msg);
        }

        public void Dispose()
        {
            EventMsgSended = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
    }
}
