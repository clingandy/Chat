using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatTestApp.Tool;

namespace ChatTestApp.WebSocket
{
    public class WebSocketHelper
    {
        private string _channelName;
        private string _userId;
        private string _userName;
        private bool _isTest;

        public ClientWebSocket _clientWebSocket;

        public Action<string> EventReceiveMsg;  //接受消息的委托
        public Action<Exception> EventError;       //连接错误的委托
        public Action<Exception> EventTestError;   //测试连接错误的委托

        public WebSocketHelper(string channelName, string userId , string userName = "TestName", bool isTest = true)
        {
            _channelName = channelName;
            _userId = userId;
            _userName = userName;
            _isTest = isTest;
        }

        public void ConnServer()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    _clientWebSocket = new ClientWebSocket();
                    await _clientWebSocket.ConnectAsync(new Uri($"{AppConfig.WebSocketUrl}?{_channelName}?{_userId}?{_userName}"), CancellationToken.None);

                    while (true)
                    {
                        var buffer = new ArraySegment<byte>(new byte[1024 * 5]);
                        await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None); //接受数据
                        var msgStr = Encoding.UTF8.GetString(Utils.RemoveSeparator(buffer.ToArray()));

                        var handeler = EventReceiveMsg;
                        handeler?.Invoke(msgStr);
                    }

                }
                catch (Exception e)
                {
                    // 并发测试的不在重连
                    if (_isTest)
                    {
                        var handeler = EventTestError;
                        handeler?.Invoke(e);
                    }
                    else
                    {
                        var handeler = EventError;
                        handeler?.Invoke(e);
                        //重连
                        Thread.Sleep(5000);
                        ConnServer();
                    }
                }
            }, TaskCreationOptions.LongRunning);

            
        }

        public void SendMsg(string msg)
        {
            //WebSocket发送消息
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
            _clientWebSocket?.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
