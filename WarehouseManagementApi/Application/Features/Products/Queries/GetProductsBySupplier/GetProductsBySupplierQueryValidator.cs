using FluentValidation;

namespace Application.Features.Products.Queries.GetProductsBySupplier;

public class GetProductsBySupplierQueryValidator
    : AbstractValidator<GetProductsBySupplierQuery>
{
    public GetProductsBySupplierQueryValidator()
    {
        RuleFor(p => p.SupplierName)
            .NotEmpty()
            .WithMessage("Supplier name is required");
    }
}