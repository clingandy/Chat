using System.Threading;
using System.Threading.Tasks;
using ChatWeb.WebSocket;
using Microsoft.Extensions.Hosting;

namespace ChatWeb
{
    public class AppBackgroundService : BackgroundService
    {
        private readonly ChatService _chatService;

        public AppBackgroundService(ChatService webSocketService)
        {
            _chatService = webSocketService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(_chatService.InitWebSocker, stoppingToken);
        }
    }
}
