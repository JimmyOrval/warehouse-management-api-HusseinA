using FluentValidation;

namespace Application.Features.WarehouseItems.Commands.CreateWarehouseItem;

public class CreateWarehouseItemCommandValidator : AbstractValidator<CreateWarehouseItemCommand>
{
    public CreateWarehouseItemCommandValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product ID  is required")
            .Length(36)
            .WithMessage("Product ID format invalid");

        RuleFor(item => item.Location)
            .NotEmpty()
            .WithMessage("Location is required");
    }
}