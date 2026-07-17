using FluentValidation;

namespace Application.Features.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceCommandValidator
    : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.")
            .Length(36)
            .WithMessage("Product ID format invalid");
        
        RuleFor(p => p.NewPrice)
            .NotEmpty()
            .WithMessage("New Price is required")
            .GreaterThan(0)
            .WithMessage("New Price cannot be negative");
    }
}