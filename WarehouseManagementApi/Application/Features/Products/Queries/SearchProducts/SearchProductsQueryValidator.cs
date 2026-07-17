using FluentValidation;

namespace Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryValidator
    : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(p => p.Name)
            .MaximumLength(50)
            .WithMessage("Product name cannot exceed 50 characters");
        
        RuleFor(p => p.Supplier)
            .MaximumLength(100)
            .WithMessage("Supplier name cannot exceed 100 characters");
    }
}