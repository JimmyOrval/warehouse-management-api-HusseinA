using FluentValidation;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryValidator
    : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
    }
}