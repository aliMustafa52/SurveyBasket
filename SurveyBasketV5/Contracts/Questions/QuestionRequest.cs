namespace SurveyBasketV5.Contracts.Questions
{
    public record QuestionRequest
    (
        string Content,
        List<string> Answers
    );
}
