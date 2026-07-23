using FluentValidation;

namespace Application.Features.WarehouseItems.Commands.UpdateProductQuantity;

public class AdjustItemQuantityCommandValidator
    : AbstractValidator<AdjustItemQuantityCommand>
{
    public AdjustItemQuantityCommandValidator()
    {
        RuleFor(p => p.ItemId)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");

        RuleFor(q => q.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required");
    }
}