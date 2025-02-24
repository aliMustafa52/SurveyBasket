namespace SurveyBasketV5.Contracts.Authentication
{
    public record ResetPasswordRequest
    (
        string Email,
        string Code,
        string NewPassword
    );
}
