using System;
using System.ComponentModel.DataAnnotations;
using Identity.Data.Models;

namespace Identity.Data.Dtos.Request.Account
{
    public class Profile
    {
        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Country { get; set; }
    }
}