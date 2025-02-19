namespace SurveyBasketV5.Contracts.Results.PollsRawData
{
    public record PollVotesResponse
    (
        string Title,
        IEnumerable<VoteResponse> Votes
    );
}
