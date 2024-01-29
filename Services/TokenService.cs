using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Models;
using Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly DataContext _context;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, DataContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public TokenResponse GenerateToken(Tuple<User, Role> userDetails)
        {
            var user = userDetails.Item1;
            var role = userDetails.Item2;

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Email),
                string.IsNullOrEmpty(role.Name ) ? null :
                new Claim(ClaimTypes.Role, role.Name),
                new Claim(ClaimTypes.NameIdentifier, user.CustomerId),
                string.IsNullOrEmpty(user.LastName) ? null :
                new Claim(ClaimTypes.Surname, user.LastName),
                string.IsNullOrEmpty(user.FirstName) ? null :
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Country, user.Country)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims,
                expires: DateTime.Now.AddMinutes(Double.Parse(_configuration["Jwt:ExpiryDurationMinutes"])), signingCredentials: credentials);
            return new TokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor), RefreshToken = GenerateRefreshToken() };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task<string> GenerateToken(User user)
        {
            string token = RandomString();
            UserToken userToken = new UserToken
            {
                Token = token,
                UserId = user.Id
            };

            _context.Add(userToken);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<bool> VerifyToken(User user, string token)
        {
            bool isValid = false;
            UserToken userToken = await _context.UserTokens
                .Include(u => u.User)
                .Where(u => u.User.Id.Equals(user.Id))
                .FirstOrDefaultAsync();
            if (userToken != null)
                isValid = userToken.Token.Equals(token);
            return isValid;
        }

        private string RandomString()
        {
            string randomString = string.Empty;
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[64];
                cryptoProvider.GetBytes(bytes);
                randomString = Convert.ToBase64String(bytes);
            }
            return randomString;
        }
    }
}