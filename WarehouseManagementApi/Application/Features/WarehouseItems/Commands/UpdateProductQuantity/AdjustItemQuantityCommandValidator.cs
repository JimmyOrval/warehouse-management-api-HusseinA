using FluentValidation;

namespace Application.Features.WarehouseItems.UpdateProductQuantity;

public class AdjustItemQuantityCommandValidator
    : AbstractValidator<AdjustItemQuantityCommand>
{
    public AdjustItemQuantityCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");

        RuleFor(q => q.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required");

        RuleFor(p => p.Location)
            .NotEmpty()
            .WithMessage("Location is required");
    }
}