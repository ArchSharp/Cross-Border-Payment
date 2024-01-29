using System;

namespace Identity.Data.Dtos.Response.Zai
{
    public class ZaiAccount
    {
        public Guid Id { get; set; }
        public string VirtualAccountId { get; set; }
        public string TopRateUserId { get; set; }
        public string ZaiWalletAccountId { get; set; }
        public string ZaiAccountNumber { get; set; }
        public string PayId { get; set; }
        public string PayIdReference { get; set; }
        public string BPayReference { get; set; }
        public string Status { get; set; }
    }
}