using FluentValidation;
using smartquote.api.DTOs.Account;

namespace smartquote.api.Validators;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.AccessToken)
            .NotEmpty().NotNull().WithMessage("Access token is required.");
    }
}
