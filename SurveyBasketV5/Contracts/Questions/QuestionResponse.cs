using SurveyBasketV5.Contracts.Answers;

namespace SurveyBasketV5.Contracts.Questions
{
    public record QuestionResponse
    (
        int Id,
        string Content,
        IEnumerable<AnswerResponse> Answers
    );
}
