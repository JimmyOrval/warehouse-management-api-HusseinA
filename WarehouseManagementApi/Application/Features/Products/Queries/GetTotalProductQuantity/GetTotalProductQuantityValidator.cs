using Application.Features.Products.Queries.GetProductById;
using FluentValidation;

namespace Application.Features.Products.Queries.GetTotalProductQuantity;

public class GetTotalProductQuantityValidator
    : AbstractValidator<GetProductByIdQuery>
{
    public GetTotalProductQuantityValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
    }
}