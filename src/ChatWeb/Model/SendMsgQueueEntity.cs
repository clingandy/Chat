using System.Collections.Generic;
using Fleck;

namespace ChatWeb.Model
{
    public class SendMsgQueueSingleEntity
    {
        public IWebSocketConnection Socket { get; set; }
        public string Msg { get; set; }
    }

    public class SendMsgQueueBatchEntity
    {
        public List<IWebSocketConnection> SocketList { get; set; }
        public string Msg { get; set; }
    }
}
