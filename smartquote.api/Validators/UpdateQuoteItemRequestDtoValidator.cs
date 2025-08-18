using FluentValidation;
using smartquote.api.DTOs.Items.Requests;

namespace smartquote.api.Validators;

public class UpdateQuoteItemRequestDtoValidator : AbstractValidator<UpdateQuoteItemRequestDto>
{
    public UpdateQuoteItemRequestDtoValidator()
    {
        RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Item Id must be greater than 0");

        RuleFor(x => x.QuoteId)
            .GreaterThan(0).WithMessage("Quote Id must be greater than 0");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode is required")
            .MaximumLength(50).WithMessage("Barcode must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
