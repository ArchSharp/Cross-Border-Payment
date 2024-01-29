using System.ComponentModel.DataAnnotations;
using Identity.Data.Dtos.Request.Auth.Login;

namespace Identity.Data.Dtos.Request.Auth
{
    public class CreateUser : PasswordLogin
    {

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(2)]
        public string Country { get; set; }

        public string ReferalCode { get; set; }
    }
}