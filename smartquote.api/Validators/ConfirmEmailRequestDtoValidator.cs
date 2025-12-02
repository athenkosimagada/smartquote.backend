using FluentValidation;
using smartquote.api.DTOs.Account;

namespace smartquote.api.Validators;

public class ConfirmEmailRequestDtoValidator : AbstractValidator<ConfirmEmailRequestDto>
{
    public ConfirmEmailRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().NotNull().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Token)
            .NotEmpty().NotNull().WithMessage("Confirmation code is required.")
            .Matches("^[0-9]+$").WithMessage("Confirmation code must contain numbers only.");
    }
}
