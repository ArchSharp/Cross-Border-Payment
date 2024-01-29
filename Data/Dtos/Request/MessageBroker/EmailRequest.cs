using System.Collections.Generic;

namespace Identity.Data.Dtos.Request.MessageBroker
{
    public class EmailRequest
    {
        public string EmailSubject { get; set; }
        public string ReceiverEmail { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}