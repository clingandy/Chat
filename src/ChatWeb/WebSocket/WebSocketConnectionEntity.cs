using System;
using Fleck;

namespace ChatWeb.WebSocket
{
    public class WebSocketConnectionEntity : IDisposable
    {
        public WebSocketConnectionEntity(IWebSocketConnection iWebSocketConnection)
        {
            _iWebSocketConnection = iWebSocketConnection;
        }

        public IWebSocketConnection _iWebSocketConnection;

        public void Dispose()
        {
            
        }
    }
}
