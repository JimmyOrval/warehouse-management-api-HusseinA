using FluentValidation;

namespace Application.Features.Products.Commands.ArchiveProduct;

public class ArchiveProductCommandValidator
    : AbstractValidator<ArchiveProductCommand>
{
    public ArchiveProductCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
    }
}