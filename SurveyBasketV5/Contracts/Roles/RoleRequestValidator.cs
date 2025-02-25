using FluentValidation;

namespace SurveyBasketV5.Contracts.Roles
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 200);

            RuleFor(x => x.Permissions)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Permissions)
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("Can't add the duplicated permission for the same role")
                .When(x => x.Permissions != null);


        }
    }
}
