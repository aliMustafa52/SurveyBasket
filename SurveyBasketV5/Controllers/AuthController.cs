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
            throw new Exception("Ali Mustafa");
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value) 
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
    }
}
