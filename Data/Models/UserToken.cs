using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Data.Models
{
    public class UserToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string Token { get; set; }
    }
}