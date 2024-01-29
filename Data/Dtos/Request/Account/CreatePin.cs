using System.ComponentModel.DataAnnotations;

namespace Identity.Data.Dtos.Request.Account
{
    public class CreatePin
    {
        [Required]
        [MaxLength(6)]
        [MinLength(6)]
        public string Pin { get; set; }
    }
}