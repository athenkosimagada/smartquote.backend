using FluentValidation;
using smartquote.api.DTOs.Quotes.Requests;

namespace smartquote.api.Validators;

public class UpdateQuoteRequestDtoValidator : AbstractValidator<UpdateQuoteRequestDto>
{
    public UpdateQuoteRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CustomerName)
            .NotEmpty().NotNull().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");
    }
}
