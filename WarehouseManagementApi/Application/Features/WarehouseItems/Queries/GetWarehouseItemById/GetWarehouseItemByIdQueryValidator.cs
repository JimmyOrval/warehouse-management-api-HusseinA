using FluentValidation;

namespace Application.Features.WarehouseItems.Queries.GetWarehouseItemById;

public class GetWarehouseItemByIdQueryValidator
    : AbstractValidator<GetWarehouseItemByIdQuery>
{
    public GetWarehouseItemByIdQueryValidator()
    {
        RuleFor(p => p.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required")
            .Length(36)
            .WithMessage("Item ID format invalid");
    }
}