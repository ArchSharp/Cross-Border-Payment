namespace Identity.Data.Dtos.Request.User
{
    public class StaffPaginationFilter : PaginationFilter
    {
        public string Position { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}