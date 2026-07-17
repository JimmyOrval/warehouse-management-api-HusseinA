using FluentValidation;

namespace Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public  CreateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(50)
            .WithMessage("Product name cannot exceed 50 characters");
        
        RuleFor(p => p.Sku)
            .NotEmpty()
            .WithMessage("Product SKU is required")
            .MaximumLength(50)
            .WithMessage("Product SKU cannot exceed 50 characters");
        
        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Product description is required")
            .MaximumLength(1000)
            .WithMessage("Product description cannot exceed 1000 characters");
        
        RuleFor(p => p.Price)
            .NotEmpty()
            .WithMessage("Product price is required")
            .GreaterThan(0)
            .WithMessage("Product price cannot be negative");
        
        RuleFor(p => p.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier ID is required")
            .Length(36)
            .WithMessage("Supplier ID format invalid");
        
        RuleFor(p => p.ExpiryDate)
            .NotEmpty()
            .WithMessage("Expiry date is required")
            .GreaterThan(DateTime.Now)
            .WithMessage("Expiry date must be in the future");
    }
}