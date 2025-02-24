namespace SurveyBasketV5.Contracts.Users
{
    public record ChangePasswordRequest
    (
        string CurrentPassword,
        string NewPassword
    );
}
