namespace SurveyBasketV5.Contracts.Polls
{
    public record PollRequest
    (
        string Title,
        string Summary,
        DateOnly StartsAt,
        DateOnly EndsAt
    );
}
