using FluentValidation;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandValidator
    : AbstractValidator<UpdateProductQuantityCommand>
{
    public UpdateProductQuantityCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
        
        RuleFor(q => q.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(p => p.Location)
            .NotEmpty()
            .WithMessage("Location is required");
    }
}