namespace SurveyBasketV5.Contracts.Votes
{
    public record VoteRequest
    (
        IEnumerable<VoteAnswerRequest> VoteAnswers
    );
}
