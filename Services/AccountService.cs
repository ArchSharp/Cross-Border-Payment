using System;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Data.Dtos.Response.Auth;
using Identity.Data.Models;
using Identity.Interfaces;
using Profile = Identity.Data.Dtos.Request.Account.Profile;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountService(IUserService userService, INotificationService notificationService, IMapper mapper, ITokenService tokenService)
        {
            _userService = userService;
            _notificationService = notificationService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<User> CreatePin(string pin, string email)
        {
            pin = BCrypt.Net.BCrypt.HashPassword(pin);
            User user = await _userService.Pin(pin, email);
            _notificationService.SendPinAddEmail(user.Email, user.FirstName);
            return user;
        }

        public async Task<bool> HasPin(string email) => await _userService.HasPin(email);

        public async Task<User> UpdatePassword(string password, string email)
        {
            password = BCrypt.Net.BCrypt.HashPassword(password);
            User user = await _userService.UpdatePassword(password, email);
            _notificationService.SendPinUpdateEmail(user.Email, user.FirstName);
            return user;
        }

        public async Task<Tuple<User, string>> ChangePassword(string password, string newPassword, string email)
        {
            var user = await _userService.User(email);
            var compare = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (compare)
            {
                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user = await _userService.UpdatePassword(newPassword, email);
                _notificationService.SendPinUpdateEmail(user.Email, user.FirstName);
                return new Tuple<User, string>(user, "Password updated");
            }
            return new Tuple<User, string>(null, "Password doest not match");
        }

        public async Task<User> UpdatePin(string pin, string email)
        {
            pin = BCrypt.Net.BCrypt.HashPassword(pin);
            User user = await _userService.Pin(pin, email);
            _notificationService.SendPinUpdateEmail(user.Email, user.FirstName);
            return user;
        }

        public async Task<Tuple<AuthResponse, string>> UpdateProfile(Profile profile, string email)
        {
            AuthResponse response = null;
            User user = await _userService.User(email);

            if (user != null)
            {
                if (!user.IsEmailVerified)
                    return new Tuple<AuthResponse, string>(response, "Please verify your email to proceed!");

                user.Address = profile.Address;
                user.City = profile.City;
                user.DateOfBirth = profile.DateOfBirth;
                user.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : profile.FirstName;
                user.LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName : profile.LastName;
                user.MiddleName = !string.IsNullOrEmpty(user.MiddleName) ? user.MiddleName : profile.MiddleName;
                user.Country = !string.IsNullOrEmpty(user.Country) ? user.Country : profile.Country;
                user.PhoneNumber = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : profile.PhoneNumber;
                user.State = profile.State;
                user.Gender = profile.Gender;
                await _userService.UpdateProfile(user);

                var role = await _userService.UserRole(user.Email);
                TokenResponse token = _tokenService.GenerateToken(new System.Tuple<User, Role>(user, role));
                var updatedProfile = _mapper.Map<UserProfile>(user);
                updatedProfile.Role = role.Name;
                response = new AuthResponse
                {
                    Profile = updatedProfile,
                    TokenResponse = token
                };
                await _userService.AddRefereshToken(user, token.RefreshToken);
            }
            return new Tuple<AuthResponse, string>(response, string.Empty);
        }

        public async Task<Tuple<User, bool>> ConfirmEmail(string token, string email)
        {
            bool isValid = false;
            var user = await _userService.User(email);
            if (user != null)
            {
                isValid = await _tokenService.VerifyToken(user, token);
                if (isValid)
                {
                    user.IsEmailVerified = true;
                    await _userService.Update(user);
                }
            }
            return new Tuple<User, bool>(user, isValid);
        }

        public async Task<bool> IsExist(string email) => await _userService.Exist(email);
    }
}