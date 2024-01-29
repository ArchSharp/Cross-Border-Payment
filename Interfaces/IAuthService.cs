using System;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.Auth.Login;
using Identity.Data.Dtos.Response.Auth;

namespace Identity.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Register(CreateUser user);
        Task<bool> Exist(string email);
        Task<Tuple<AuthResponse, string>> AuthenticatePassword(PasswordLogin user);
        Task<Tuple<AuthResponse, string>> AuthenticatePin(PinLogin user);
        Task<string> ForgotPassword(string email);
        Task<Tuple<AuthResponse, string>> Referesh(TokenResponse tokenResponse);
    }
}