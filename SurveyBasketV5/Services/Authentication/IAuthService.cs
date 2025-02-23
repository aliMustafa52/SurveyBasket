namespace SurveyBasketV5.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
        Task<Result<AuthResponse>> GetRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
