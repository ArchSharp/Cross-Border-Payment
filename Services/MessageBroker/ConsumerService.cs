using System;
using System.Text;
using System.Threading.Tasks;
using Identity.Interfaces;
using Identity.Interfaces.MessageBroker;
using Identity.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Identity.Services
{



    public class ConsumerService : IConsumerService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly ILogger _logger;

        public ConsumerService(IConfiguration configuration, IRabbitMQConfig rabbitMQConfig, ILogger<ConsumerService> logger)
        {
            _configuration = configuration;
            _connection = rabbitMQConfig.CreateChannel(true);
            _logger = logger;
        }

        public void RecieveMessage(string queue)
        {
            var channel = _connection.CreateModel();
            channel = ConfigureChannel(channel, queue);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await HandleMessage(message);
                channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            channel.BasicConsume(queue, false, consumer);
        }

        private Task HandleMessage(string message)
        {
            // JObject json = JObject.Parse(message);
            // string type = json.Value<string>("Type").ToLower();
            // switch (type)
            // {
            //     case "verified":
            //         var verificationPayload = JsonConvert.DeserializeObject<MessageBrokerResquest<VerifiedUser>>(message);
            //         _userService.AttemptVerification(verificationPayload.Data);
            //         break;
            //     default:
            //         break;
            // }
            throw new NotImplementedException();
        }

        private IModel ConfigureChannel(IModel channel, string queue)
        {
            string routingKey = _configuration["MessageBroker:QUEUE_IDENTITY_ROUTING_KEY"];
            string exchange = _configuration["MessageBroker:QUEUE_IDENTITY_EXCHANGE"];
            channel.ExchangeDeclare(exchange, ExchangeType.Topic);
            channel.QueueDeclare(queue, false, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);
            channel.BasicQos(0, 1, false);
            return channel;
        }

        public void Dispose()
        {
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}