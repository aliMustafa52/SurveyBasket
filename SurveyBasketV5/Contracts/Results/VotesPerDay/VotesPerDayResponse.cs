namespace SurveyBasketV5.Contracts.Results.VotesPerDay
{
    public record VotesPerDayResponse
    (
        DateOnly Date,
        int NumberOfVotes
    );
}
