using FluentValidation;
using smartquote.api.DTOs.Account;

namespace smartquote.api.Validators;

public class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset password code is required.")
            .Matches("^[0-9]+$").WithMessage("Reset password code must contain numbers only.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().NotNull().WithMessage("New Password is required.")
            .MinimumLength(8).WithMessage("New Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("New Password cannot exceed 100 characters.")
            .Matches(@"[A-Z]").WithMessage("New Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("New Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("New Password must contain at least one digit.")
            .Matches(@"[\W_]").WithMessage("New Password must contain at least one special character.");
    }
}
