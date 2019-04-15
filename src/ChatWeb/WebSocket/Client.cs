using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ChatWeb.Enum;
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

        public bool IsClose { get; set; }

        public ClientStatusEnum Status { get; set; } = ClientStatusEnum.OnLine;

        public IWebSocketConnection Socket { get; set; }

        public event Action<IClient, MsgEntity> EventMsgSended;

        private DateTime _nextSendMsgTime = DateTime.Now;

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

        /// <summary>
        /// 登出用
        /// </summary>
        public Client(string clientId, ClientStatusEnum status)
        {
            ClientId = clientId;
            Status = status;
        }

        public void MsgSend(MsgEntity msg)
        {
            //if (_nextSendMsgTime > DateTime.Now)
            //{
            //    msg.Type = (int)MsgTypeEnum.禁止发送;
            //}
            //else
            //{
            //    _nextSendMsgTime = DateTime.Now.AddSeconds(1);
            //}

            msg.FromId = ClientId;
            msg.FromName = ClientName;
            msg.CurTime = DateTime.Now.ConvertDateTimeToInt();
            var handler = EventMsgSended;
            handler?.Invoke(this, msg);
        }

        public async Task MsgReceive(string msg)
        {
            if (!IsClose || Socket.IsAvailable)
            {
                await Socket.Send(msg);
            }
        }

        public void Dispose()
        {
            EventMsgSended = null;
        }
    }
}
