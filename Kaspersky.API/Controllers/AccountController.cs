using Kaspersky.Infrastructure.Entity.Login;
using Kaspersky.Infrastructure.Entity.RefreshToken;
using Kaspersky.Infrastructure.Entity.SignUp;
using Kaspersky.Infrastructure.InterfaceService;
using Kaspersky.Infrastructure.ResponseService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaspersky.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync(SignUpModel model)
        {
            var response = await _accountService.SignUpAsync(model);

            if (response.Status == ResponseConst.Success)
            {
                // Return Ok for successful sign-up attempts
                return Ok(response);
            }
            return Unauthorized(response);
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync(LoginModel model)
        {
            var response = await _accountService.SignInAsync(model);

            if (response.Status == ResponseConst.Error)
            {
                // Return Unauthorized for failed sign-in attempts
                return Unauthorized(new CommonResponseBase
                {
                    Status = ResponseConst.Error,
                    Message = LoginConst.UserDoesnotExist
                });
            }
            return Ok(response);
        }

        [HttpGet("GetInfo/{userId}")]
        public async Task<IActionResult> GetInfo(string userId)
        {
            var result = await _accountService.GetInfo(userId);

            return Ok(result);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshTokenRequestViewModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid refresh token request");
            }

            var response = await _accountService.RefreshAccessToken(request.RefreshToken);

            if (response.Status == ResponseConst.Error)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
