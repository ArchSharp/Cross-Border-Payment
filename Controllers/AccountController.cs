
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Account;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.Auth;
using Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Identity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accounService;
        private readonly IConfiguration _configuration;
        private readonly ITwilioService _twilioService;

        public AccountController(IAccountService accountService, IConfiguration configuration, ITwilioService twilioService)
        {
            _accounService = accountService;
            _configuration = configuration;
            _twilioService = twilioService;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Profile profile)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string email = identity.FindFirst(ClaimTypes.Name).Value;
            var response = await _accounService.UpdateProfile(profile, email);
            if (response.Item1 == null)
            {
                string message = string.IsNullOrEmpty(response.Item2) ? "Failed to update profile!" : response.Item2;
                return BadRequest(new BaseResponse<string>(message, HttpStatusCode.BadRequest, message));
            }
            return Ok(new BaseResponse<AuthResponse>(response.Item1, HttpStatusCode.OK, "Profile updated succesfully"));
        }

        //send OTP code to users
        [HttpPost("TwilioSendOTP")]
        //[ActionName("TwilioSendOTP")]
        public async Task<IActionResult> TwilioSendMessageAync([FromBody] CreateOTP model)
        {
           
            var result = await _twilioService.TwilioSendAsync(model.Message, model.To);

            if (result == null)
            { 
                return BadRequest(new BaseResponse<string>("Failed to send OTP!", HttpStatusCode.BadRequest, "Failed to send OTP!"));
            }

            return Ok(result);
        }

        [HttpPost("pin")]
        public async Task<IActionResult> CreatePin([FromBody] CreatePin createPin)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string email = identity.FindFirst(ClaimTypes.Name).Value;
            bool hasPin = await _accounService.HasPin(email);
            if (hasPin)
                return Ok(new BaseResponse<string>("Pin already set!", HttpStatusCode.OK, "Pin already set!"));
            var response = await _accounService.CreatePin(createPin.Pin, email);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to set pin!", HttpStatusCode.BadRequest, "Failed to set pin!"));
            return Ok(new BaseResponse<string>("Pin set successfully", HttpStatusCode.OK, "Pin set succesfully"));
        }

        [HttpPut("pin")]
        public async Task<IActionResult> UpdatePin([FromBody] CreatePin createPin)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string email = identity.FindFirst(ClaimTypes.Name).Value;
            bool hasPin = await _accounService.HasPin(email);
            if (!hasPin)
                return BadRequest(new BaseResponse<string>("You cannot update pin", HttpStatusCode.BadRequest, "You cannot update pin"));
            var response = await _accounService.UpdatePin(createPin.Pin, email);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to set pin!", HttpStatusCode.BadRequest, "Failed to set pin!"));
            return Ok(new BaseResponse<string>("Pin set successfully", HttpStatusCode.OK, "Pin set succesfully"));
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword password)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string email = identity.FindFirst(ClaimTypes.Name).Value;
            var response = await _accounService.UpdatePassword(password.Password, email);
            if (response == null)
                return BadRequest(new BaseResponse<string>("Failed to update password!", HttpStatusCode.BadRequest, "Failed to update password!"));
            return Ok(new BaseResponse<string>("Password updated successfully", HttpStatusCode.OK, "Password updated succesfully"));
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword password)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string email = identity.FindFirst(ClaimTypes.Name).Value;
            bool isExist = await _accounService.IsExist(email);
            if (!isExist)
                return BadRequest(new BaseResponse<string>("Failed to change password!", HttpStatusCode.BadRequest, "Failed to change password!"));

            var response = await _accounService.ChangePassword(password.Password, password.NewPassword, email);
            if (response == null || response.Item1 == null)
                return BadRequest(new BaseResponse<string>("Failed to update password!", HttpStatusCode.BadRequest, response?.Item2));
            return Ok(new BaseResponse<string>("Password updated successfully", HttpStatusCode.OK, "Password updated succesfully"));
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var user = await _accounService.ConfirmEmail(token, email);
                return Redirect(_configuration["BASE_URL"]);
            }
            return BadRequest(new BaseResponse<string>("Failed to confirm email!", HttpStatusCode.BadRequest, "Failed to confirm email!"));
        }
    }
}