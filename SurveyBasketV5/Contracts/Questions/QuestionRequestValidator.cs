using FluentValidation;

namespace SurveyBasketV5.Contracts.Questions
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(3, 1000);

            RuleFor(x => x.Answers)
                .Must(x => x.Count > 1)
                .WithMessage("Question should have At least two Answers")
                .When(x => x.Answers is not null);

            RuleFor(x => x.Answers)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You cannot add duplicated answers")
                .When(x => x.Answers is not null);


        }
    }
}
