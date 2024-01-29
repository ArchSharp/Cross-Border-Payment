using System.Collections.Generic;
using System.Web;
using Identity.Data.Dtos.Request.MessageBroker;
using Identity.Interfaces;
using Identity.Interfaces.MessageBroker;
using Microsoft.Extensions.Configuration;

namespace Identity.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IProducerService _producerService;
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration, IProducerService producerService)
        {
            _configuration = configuration;
            _producerService = producerService;
        }

        public void SendPasswordResetEmail(string receiverEmail, string name, string token)
        {
            EmailRequest request = new EmailRequest();
            request.ReceiverEmail = receiverEmail;
            request.EmailSubject = "Forget Password";
            request.TemplateName = "Reset";

            string link = _configuration["URLS:reset"];
            token = HttpUtility.UrlEncode(token);
            link = link.Replace("[token]", token);
            request.Variables = new Dictionary<string, string>(){
                {"name", name},
                {"link", link}
            };
            var message = new Notification<EmailRequest>()
            {
                Type = "Email",
                Data = request
            };
            _producerService.SendMessage(message, _configuration["MessageBroker:QUEUE_NOTIFICATION"]);
        }

        public void SendPinAddEmail(string email, string firstName)
        {
            // throw new System.NotImplementedException();
        }

        public void SendPinUpdateEmail(string email, string firstName)
        {
            // throw new System.NotImplementedException();s
        }

        public void SendStaffAccountEmail(string email, string firstName) { }

        public void SendVerificationEmail(string receiverEmail, string name, string token)
        {
            EmailRequest request = new EmailRequest();
            request.ReceiverEmail = receiverEmail;
            request.EmailSubject = "Email verification";
            request.TemplateName = "Welcome";

            string link = _configuration["URLS:verify"];
            token = HttpUtility.UrlEncode(token);
            link = link.Replace("[email]", request.ReceiverEmail).Replace("[token]", token);
            request.Variables = new Dictionary<string, string>(){
                {"name", name},
                {"link", link}
            };
            var message = new Notification<EmailRequest>()
            {
                Type = "Email",
                Data = request
            };
            _producerService.SendMessage(message, _configuration["MessageBroker:QUEUE_NOTIFICATION"]);
        }

        public void Subscribe(string receiverEmail, string firstName, string lastName)
        {
            Subscriber subscriber = new Subscriber()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = receiverEmail
            };
            var message = new Notification<Subscriber>()
            {
                Type = "Subscribe",
                Data = subscriber
            };
            _producerService.SendMessage(message, _configuration["MessageBroker:QUEUE_NOTIFICATION"]);
        }
    }
}