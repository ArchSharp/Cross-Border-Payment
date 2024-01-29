using System.ComponentModel.DataAnnotations;

namespace Identity.Data.Dtos.Request.Auth
{
    public class BaseUser
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}