namespace SurveyBasketV5.Contracts.Authentication
{
    public record ConfirmEmailRequest
    (
        string UserId,
        string Code
    );
}
