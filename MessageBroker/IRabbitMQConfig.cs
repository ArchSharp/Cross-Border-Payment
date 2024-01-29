
using RabbitMQ.Client;

namespace Identity.MessageBroker
{
    public interface IRabbitMQConfig
    {
        IConnection CreateChannel(bool async);
    }
}