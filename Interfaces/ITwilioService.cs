using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

namespace Identity.Interfaces
{
    public interface ITwilioService
    {
        Task<MessageResource> TwilioSendAsync(string message, string to);
    }
}
