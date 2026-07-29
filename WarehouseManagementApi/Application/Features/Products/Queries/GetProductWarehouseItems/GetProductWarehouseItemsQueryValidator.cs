using FluentValidation;

namespace Application.Features.Products.Queries.GetProductWarehouseItems;

public class GetProductWarehouseItemsQueryValidator
    : AbstractValidator<GetProductWarehouseItemsQuery>
{
    public GetProductWarehouseItemsQueryValidator()
    {
        RuleFor(p => p.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
    }
}