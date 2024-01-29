using System.ComponentModel.DataAnnotations;

namespace Identity.Data.Dtos.Request.Auth.Login
{
    public class PinLogin : BaseUser
    {
        [Required]
        public string Pin { get; set; }
    }
}