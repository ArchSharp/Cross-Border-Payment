using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Models;

namespace Identity.Interfaces
{
    public interface ITokenService
    {
        TokenResponse GenerateToken(Tuple<User, Role> userDetails);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<string> GenerateToken(User user);
        Task<bool> VerifyToken(User user, string token);
    }
}