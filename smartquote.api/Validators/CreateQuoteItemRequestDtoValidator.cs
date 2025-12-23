using FluentValidation;
using smartquote.api.DTOs.Items.Requests;

namespace smartquote.api.Validators;

public class CreateQuoteItemRequestDtoValidator : AbstractValidator<CreateQuoteItemRequestDto>
{
    public CreateQuoteItemRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Barcode)
            .NotEmpty().NotNull().WithMessage("Barcode is required")
            .MaximumLength(50).WithMessage("Barcode must not exceed 50 characters");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
