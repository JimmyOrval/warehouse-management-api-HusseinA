using Application.Features.WarehouseItems.Queries.GetWarehouseItemById;
using FluentValidation;

namespace Application.Features.WarehouseItems.Queries.GetItemStockMovements;

public class GetItemStockMovementsQueryValidator
    : AbstractValidator<GetItemStockMovementsQuery>
{
    public GetItemStockMovementsQueryValidator()
    {
        RuleFor(p => p.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required")
            .Length(36)
            .WithMessage("Item ID format invalid");
    }
}