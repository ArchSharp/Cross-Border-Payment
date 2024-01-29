using System;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Account;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Models;

namespace Identity.Interfaces
{
    public interface IAccountService
    {
        Task<bool> HasPin(string email);
        Task<User> CreatePin(string pin, string email);
        Task<User> UpdatePin(string pin, string email);
        Task<User> UpdatePassword(string password, string email);
        Task<Tuple<User, string>> ChangePassword(string password, string newPassword, string email);
        Task<Tuple<AuthResponse, string>> UpdateProfile(Profile profile, string email);
        Task<Tuple<User, bool>> ConfirmEmail(string token, string email);
        Task<bool> IsExist(string email);
    }
}