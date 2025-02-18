namespace SurveyBasketV5.Contracts.Votes
{
    public record VoteAnswerRequest
    (
        int QuestionId,
        int AnswerId
    );
}
