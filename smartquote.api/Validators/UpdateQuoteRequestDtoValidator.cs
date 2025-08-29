using FluentValidation;
using smartquote.api.DTOs.Quotes.Requests;

namespace smartquote.api.Validators;

public class UpdateQuoteRequestDtoValidator : AbstractValidator<UpdateQuoteRequestDto>
{
    public UpdateQuoteRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId)
            .NotEmpty().NotNull().WithMessage("User ID is required.")
            .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID.");

        RuleFor(x => x.Customer)
            .NotEmpty().NotNull().WithMessage("Customer is required.")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");
    }

    private bool BeAValidGuid(string userId)
    {
        return Guid.TryParse(userId, out _);
    }
}
