using System;

namespace Identity.Data.Dtos.Response.Location
{
    public class LocationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public string DialingCode { get; set; }
        public bool IsReceiving { get; set; }
        public bool IsSending { get; set; }
        public Continent Continent { get; set; }
        public string Flag { get; set; }
    }
}