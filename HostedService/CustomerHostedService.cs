using System.Threading;
using System.Threading.Tasks;
using Identity.Interfaces.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.HostedService
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IConsumerService _consumerService;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IConfiguration configuration, IConsumerService consumerService)
        {
            _logger = logger;
            _configuration = configuration;
            _consumerService = consumerService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Background service starting...");
            _consumerService.RecieveMessage(_configuration["MessageBroker:QUEUE_IDENTITY"]);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _consumerService.Dispose();
            base.Dispose();
        }
    }
}