using FluentValidation;

namespace Application.Features.Products.Queries.GetPagedProducts;

public class GetPagedProductsQueryValidator
    : AbstractValidator<GetPagedProductsQuery>
{
    public GetPagedProductsQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .NotEmpty()
            .WithMessage("Page number is required")
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero");
        
        RuleFor(q => q.PageSize)
            .NotEmpty()
            .WithMessage("Page size is required")
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero");
    }
}