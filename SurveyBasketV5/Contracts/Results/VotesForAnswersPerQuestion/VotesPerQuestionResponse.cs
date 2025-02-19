namespace SurveyBasketV5.Contracts.Results.VotesForAnswersPerQuestion
{
    public record VotesPerQuestionResponse
    (
        string Question,
        IEnumerable<VotesPerAnswer> VotesPerAnswers
    );
}
