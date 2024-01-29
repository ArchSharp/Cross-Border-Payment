using System.ComponentModel.DataAnnotations;
using Identity.Data.Dtos.Request.Auth.Login;

namespace Identity.Data.Dtos.Request.Auth
{
    public class CreateStaff : PasswordLogin
    {

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(2)]
        public string Country { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string RoleId { get; set; }
    }
}