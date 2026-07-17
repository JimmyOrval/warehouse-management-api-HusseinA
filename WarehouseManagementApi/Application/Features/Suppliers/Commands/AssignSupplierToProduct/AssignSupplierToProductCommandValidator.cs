using FluentValidation;

namespace Application.Features.Suppliers.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommandValidator
    : AbstractValidator<AssignSupplierToProductCommand>
{
    public AssignSupplierToProductCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");
        
        RuleFor(p => p.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier ID is required")
            .Length(36)
            .WithMessage("Supplier ID format invalid");
    }
}