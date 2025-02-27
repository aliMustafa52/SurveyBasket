using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketV5.Services.Authentication;

namespace SurveyBasketV5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value) 
                : result.ToProblem();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> Revoke(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.SendResetPasswordCodeAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordCodeAsync(request);

            return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
        }

        [HttpPost("test")]
        public IActionResult Test()
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var user = new ApplicationUser();
            var pass = passwordHasher.HashPassword(user, "Y8Pmja-6)");

            return Ok(pass);
        }
    }
}
