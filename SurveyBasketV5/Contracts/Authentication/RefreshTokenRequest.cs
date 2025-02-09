namespace SurveyBasketV5.Contracts.Authentication
{
    public record RefreshTokenRequest
    (
        string Token,
        string RefreshToken
    );
}
