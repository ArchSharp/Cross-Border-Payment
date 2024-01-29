

using System;

namespace Identity.Data.Dtos.Response.Auth
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public int Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public bool IsKycVerified { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool HasPin { get; set; }
    }
}