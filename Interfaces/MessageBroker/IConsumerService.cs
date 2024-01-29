using System;

namespace Identity.Interfaces.MessageBroker
{
    public interface IConsumerService : IDisposable
    {
        public void RecieveMessage(string queue);
    }
}