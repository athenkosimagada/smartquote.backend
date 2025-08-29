using FluentValidation;
using smartquote.api.DTOs.Account;

namespace smartquote.api.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        
        RuleFor(x => x.Password)
            .NotEmpty().NotNull().WithMessage("Password is required.");
    }
}
