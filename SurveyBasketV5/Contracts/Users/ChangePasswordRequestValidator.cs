using FluentValidation;
using SurveyBasketV5.Abstractions.Consts;

namespace SurveyBasketV5.Contracts.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains lowercase, uppercase and one NonAlphanumeric");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains lowercase, uppercase and one NonAlphanumeric")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New Password should be different than old one");

            //RuleFor(x => x)
            //    .Must(x => x.CurrentPassword != x.NewPassword)
            //    .WithName("Same Password")
            //    .WithMessage("New Password should be different than old one")
            //    .When(x => !string.IsNullOrEmpty(x.CurrentPassword) && !string.IsNullOrEmpty(x.NewPassword));
        }
    }
}
