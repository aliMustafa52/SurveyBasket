using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV5.Contracts.Users;
using SurveyBasketV5.Services.Users;

namespace SurveyBasketV5.Controllers
{
    [Route("me")]
    [ApiController]
    [Authorize]
    public class AccountsController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var userId = User.GetUserId();
            var result = await _userService.GetUserProfileAsync(userId!);

            return result.IsSuccess 
                    ? Ok(result.Value) 
                    : result.ToProblem();
        }

        [HttpPut("")]
        public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetUserId();
            var result = await _userService.UpdateUserProfileAsync(userId!,request);

            return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            var result = await _userService.ChangePasswordAsync(userId!, request);

            return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
        }
    }
}
