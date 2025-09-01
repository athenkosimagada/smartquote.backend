using FluentValidation;
using smartquote.api.DTOs.Account;

namespace smartquote.api.Validators;

public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().NotNull().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
