using System.Net;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.Auth.Login;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.Auth;
using Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILocationService _locationService;
        public AuthController(IAuthService authService, ILocationService locationService)
        {
            _authService = authService;
            _locationService = locationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUser data)
        {
            bool isExist = await _authService.Exist(data.Email);
            if (isExist)
                return Conflict(new BaseResponse<string>("User already exist", HttpStatusCode.Conflict, "User already exists"));
            var locationResponse = await _locationService.Get(data.Country);
            if (locationResponse == null)
                return NotFound(new BaseResponse<string>("Country not supported", HttpStatusCode.NotFound, "Country not supported"));
            var response = await _authService.Register(data);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to create user", HttpStatusCode.BadRequest, "Failed to create user"));
            return Created("", new BaseResponse<AuthResponse>(response, HttpStatusCode.Created, "User registered succesfully"));
        }

        [HttpPost("authenticate/password")]
        public async Task<IActionResult> AuthenticatePassword([FromBody] PasswordLogin passwordLogin)
        {
            var response = await _authService.AuthenticatePassword(passwordLogin);
            if (response.Item1 == null)
                return Unauthorized(new BaseResponse<string>("Failed to login user", HttpStatusCode.Unauthorized, "Incorrect email and password"));

            if (!string.IsNullOrEmpty(response.Item2))
                return Unauthorized(new BaseResponse<string>(response.Item2, HttpStatusCode.Unauthorized, response.Item2));

            return Ok(new BaseResponse<AuthResponse>(response.Item1, HttpStatusCode.OK, "User logged in succesfully"));
        }

        [HttpPost("authenticate/pin")]
        public async Task<IActionResult> AuthenticatePin([FromBody] PinLogin passwordLogin)
        {
            var response = await _authService.AuthenticatePin(passwordLogin);
            if (response == null)
                return Unauthorized(new BaseResponse<string>("Failed to login user", HttpStatusCode.Unauthorized, "Failed to login user"));

            if (string.IsNullOrEmpty(response.Item2))
                return Unauthorized(new BaseResponse<string>(response.Item2, HttpStatusCode.Unauthorized, response.Item2));

            return Ok(new BaseResponse<AuthResponse>(response.Item1, HttpStatusCode.OK, "User logged in succesfully"));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenResponse tokenResponse)
        {
            var response = await _authService.Referesh(tokenResponse);
            if (response == null)
                return Unauthorized(new BaseResponse<string>("Invalid referesh token", HttpStatusCode.Unauthorized, "Invalid referesh token"));

            if (response.Item1 == null && string.IsNullOrEmpty(response.Item2))
                return Unauthorized(new BaseResponse<string>("Invalid referesh token", HttpStatusCode.Unauthorized, "Invalid referesh token"));

            if (!string.IsNullOrEmpty(response.Item2))
                return Unauthorized(new BaseResponse<string>(response.Item2, HttpStatusCode.Unauthorized, response.Item2));

            return Ok(new BaseResponse<AuthResponse>(response.Item1, HttpStatusCode.OK, "User logged in succesfully"));
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] BaseUser user)
        {
            var response = await _authService.ForgotPassword(user.Email);
            return Ok(new BaseResponse<string>("Please check your email for link", HttpStatusCode.OK, "Forgot password link sent successfully"));
        }
    }
}