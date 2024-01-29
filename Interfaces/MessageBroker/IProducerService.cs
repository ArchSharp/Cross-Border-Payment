namespace Identity.Interfaces.MessageBroker
{
    public interface IProducerService
    {
        void SendMessage<T>(T message, string queue);
    }
}

