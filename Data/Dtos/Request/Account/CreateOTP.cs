using System.ComponentModel.DataAnnotations;

namespace Identity.Data.Dtos.Request.Account
{
    public class CreateOTP
    {
        [MaxLength(200)]
        [Required]
        public string Message { get; set; }
        [Required]
        public string To { get; set; }
    }
}
