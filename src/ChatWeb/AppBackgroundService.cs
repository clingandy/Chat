using System.Threading;
using System.Threading.Tasks;
using ChatWeb.WebSocket;
using Microsoft.Extensions.Hosting;

namespace ChatWeb
{
    public class AppBackgroundService : BackgroundService
    {
        private readonly WebSocketService _webSocketService;

        public AppBackgroundService(WebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(_webSocketService.InitWebSocker, stoppingToken);
        }
    }
}
