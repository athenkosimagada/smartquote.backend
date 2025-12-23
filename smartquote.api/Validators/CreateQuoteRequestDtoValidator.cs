using FluentValidation;
using smartquote.api.DTOs.Quotes.Requests;

namespace smartquote.api.Validators;

public class CreateQuoteRequestDtoValidator : AbstractValidator<CreateQuoteRequestDto>
{
    public CreateQuoteRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateQuoteItemRequestDtoValidator());

    }
}
