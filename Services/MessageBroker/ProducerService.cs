using System;
using System.Text;
using System.Text.Json;
using Identity.Interfaces.MessageBroker;
using Identity.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Identity.Services.MessageBroker
{
    public class ProducerService : IProducerService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;

        public ProducerService(ILogger<ProducerService> logger, IRabbitMQConfig rabbitMQConfig, IConfiguration configuration)
        {
            _connection = rabbitMQConfig.CreateChannel(false);
            _configuration = configuration;
            _logger = logger;
        }

        public void SendMessage<T>(T message, string queue)
        {
            using var channel = _connection.CreateModel();
            string exchange = _configuration["MessageBroker:QUEUE_NOTIFICATION_EXCHANGE"];
            string routingKey = _configuration["MessageBroker:QUEUE_NOTIFICATION_ROUTING_KEY"];
            ConfigureChannel(channel, queue, exchange, routingKey);
            channel.QueueDeclare(queue, false, false, false, null);
            string json = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange, routingKey, body: body, basicProperties: ChannelProperties(channel));
        }

        private void ConfigureChannel(IModel channel, string queue, string exchange, string routingKey)
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Topic);
            channel.QueueDeclare(queue, false, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);
            channel.BasicQos(0, 1, false);
        }

        private IBasicProperties ChannelProperties(IModel channel)
        {
            var properties = channel.CreateBasicProperties();
            properties.AppId = _configuration["MessageBroker:APP_ID"];
            properties.ContentType = "application/json";
            properties.DeliveryMode = 1;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            return properties;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.IsOpen)
                _connection.Close();
        }
    }
}

