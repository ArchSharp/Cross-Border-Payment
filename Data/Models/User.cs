using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Data.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PostalCode { get; set; }
        public Gender Gender { get; set; }

        [Required]
        public string Country { get; set; }

        [MaxLength(6)]
        public string Pin { get; set; }
        public bool IsKycVerified { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool HasPin { get; set; }
        public bool IsZaiUser { get; set; }

        [ForeignKey("Role")]
        public Guid RoleId { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public virtual Role Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}