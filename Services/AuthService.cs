using System;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.Auth.Login;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Models;
using Identity.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public AuthService(IUserService userService, IMapper mapper, ITokenService tokenService, INotificationService notificationService, ILogger<AuthService> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
            _notificationService = notificationService;
            _logger = logger;
        }

        private Tuple<bool, string> CompleteAuthenticate(User user, string token)
        {
            string message = string.Empty;
            bool completeStatus = true;
            if (!user.IsEmailVerified)
            {
                message = "Please verify your email to procced";
                completeStatus = false;
                _notificationService.SendVerificationEmail(user.Email, "", token);
            }
            return new Tuple<bool, string>(completeStatus, message);
        }

        public async Task<Tuple<AuthResponse, string>> AuthenticatePassword(PasswordLogin user)
        {
            AuthResponse response = null;
            string message = string.Empty;
            var foundUser = await _userService.User(user.Email);
            if (foundUser != null)
            {
                var compare = BCrypt.Net.BCrypt.Verify(user.Password, foundUser.Password);
                if (compare)
                {
                    var role = await _userService.UserRole(foundUser.Email);
                    TokenResponse token = _tokenService.GenerateToken(new System.Tuple<User, Role>(foundUser, role));
                    var profile = _mapper.Map<UserProfile>(foundUser);
                    profile.Role = role.Name;
                    response = new AuthResponse
                    {
                        Profile = profile,
                        TokenResponse = token
                    };
                    await _userService.AddRefereshToken(foundUser, token.RefreshToken);
                    // var completeAuthenticate = CompleteAuthenticate(foundUser, token.Token);
                    // if (!completeAuthenticate.Item1 && !string.IsNullOrEmpty(completeAuthenticate.Item2))
                    //     message = completeAuthenticate.Item2;
                }
            }
            return new Tuple<AuthResponse, string>(response, message);
        }

        public async Task<Tuple<AuthResponse, string>> AuthenticatePin(PinLogin user)
        {
            AuthResponse response = null;
            string message = string.Empty;
            var foundUser = await _userService.User(user.Email);
            if (foundUser != null && foundUser.HasPin)
            {
                var compare = BCrypt.Net.BCrypt.Verify(user.Pin, foundUser.Pin);
                if (compare)
                {
                    var tokenReponse = await AttemptGenerateToken(foundUser);
                    response = tokenReponse.Item1;
                    message = tokenReponse.Item2;
                }
            }
            return new Tuple<AuthResponse, string>(response, message);
        }

        private async Task<Tuple<AuthResponse, string>> AttemptGenerateToken(User user)
        {
            AuthResponse response = null;
            string message = string.Empty;
            var role = await _userService.UserRole(user.Email);
            TokenResponse token = _tokenService.GenerateToken(new System.Tuple<User, Role>(user, role));
            var profile = _mapper.Map<UserProfile>(user);
            response = new AuthResponse
            {
                Profile = profile,
                TokenResponse = token
            };
            await _userService.AddRefereshToken(user, token.RefreshToken);
            var completeAuthenticate = CompleteAuthenticate(user, token.Token);
            if (!completeAuthenticate.Item1)
                message = completeAuthenticate.Item2;
            return new Tuple<AuthResponse, string>(response, message);
        }

        public async Task<bool> Exist(string email) => await _userService.Exist(email);

        public async Task<AuthResponse> Register(CreateUser data)
        {
            AuthResponse response = null;
            var user = _mapper.Map<User>(data);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var createResponse = await _userService.CreateUser(user);
            if (createResponse != null)
            {
                TokenResponse token = _tokenService.GenerateToken(createResponse);
                var profile = _mapper.Map<UserProfile>(createResponse.Item1);
                profile.Role = createResponse.Item2.Name;
                response = new AuthResponse
                {
                    Profile = profile,
                    TokenResponse = token
                };
                _notificationService.Subscribe(user.Email, user.FirstName, user.LastName);
                string verificationToken = await _tokenService.GenerateToken(createResponse.Item1);
                _notificationService.SendVerificationEmail(user.Email, "", verificationToken);
            }
            return response;
        }

        public async Task<string> ForgotPassword(string email)
        {
            string message = string.Empty;
            var foundUser = await _userService.User(email);
            if (foundUser != null)
            {
                var role = await _userService.UserRole(foundUser.Email);
                TokenResponse token = _tokenService.GenerateToken(new System.Tuple<User, Role>(foundUser, role));
                _notificationService.SendPasswordResetEmail(foundUser.Email, foundUser.FirstName, token.Token);
                message = "Forgot password email sent!";
            }
            return message;
        }

        public async Task<Tuple<AuthResponse, string>> Referesh(TokenResponse tokenResponse)
        {
            AuthResponse response = null;
            string message = string.Empty;
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenResponse.Token);
                var username = principal.Identity?.Name;
                var foundUser = await _userService.User(username);
                if (foundUser != null)
                {
                    bool hasToken = await _userService.CheckRefereshToken(foundUser.CustomerId, tokenResponse.RefreshToken);
                    if (hasToken)
                    {
                        var tokenReponse = await AttemptGenerateToken(foundUser);
                        response = tokenReponse.Item1;
                        message = tokenReponse.Item2;
                    }
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError("RefereshToken", ex);
            }
            return new Tuple<AuthResponse, string>(response, message);
        }
    }
}