using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Identity.MessageBroker
{
    public class RabbitMQConfig : IRabbitMQConfig
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public RabbitMQConfig(ILogger<RabbitMQConfig> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IConnection CreateChannel(bool async)
        {
            _logger.LogInformation("Connecting to RabbitMQ");
            ConnectionFactory factory = new()
            {
                HostName = _configuration["MessageBroker:RABBITMQ_HOST"],
                Password = _configuration["MessageBroker:RABBITMQ_PASSWORD"],
                UserName = _configuration["MessageBroker:RABBITMQ_USERNAME"],
                Port = int.Parse(_configuration["MessageBroker:RABBITMQ_PORT"]),
                VirtualHost = _configuration["MessageBroker:RABBITMQ_VIRTUAL"],
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = async,
            };
            return factory.CreateConnection();
        }
    }
}