namespace Identity.Data.Models
{
    public class Documents
    {
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentCountry { get; set; }
        public string DocumentValidFrom { get; set; }
        public string DocumentValidUntil { get; set; }
        public string DocumentPlaceOfIssue { get; set; }
        public string DocumentFirstIssue { get; set; }
        public string DocumentIssueNumber { get; set; }
        public string DocumentIssuedBy { get; set; }
    }
}