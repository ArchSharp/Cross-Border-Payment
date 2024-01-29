namespace Identity.Data.Dtos.Request.User
{
    public class CustomerFilter : PaginationFilter
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}