using Azure.Core;
using FluentValidation;
using System.Security.Cryptography;

namespace SurveyBasketV5.Contracts.Polls
{
    public class PollRequestValidator : AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .Length(3, 100);

            RuleFor(x => x.Summary)
                .Length(3, 1500);

            RuleFor(x => x.StartsAt)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Start date must be in the future.");

            RuleFor(x => x.EndsAt)
            .NotEmpty()
            .WithMessage("End date is required.")
            .Must((request, endsAt) => endsAt >= request.StartsAt)
            .WithMessage("End date must be greater than start date.");
        }
    }
}
