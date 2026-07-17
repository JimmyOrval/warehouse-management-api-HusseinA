using FluentValidation;

namespace Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator
    : AbstractValidator<CreateSupplierCommand>
{
    public  CreateSupplierCommandValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("Supplier name is required")
            .MaximumLength(100)
            .WithMessage("Supplier name cannot exceed 100 characters");
        
        RuleFor(s => s.Country)
            .NotEmpty()
            .WithMessage("Supplier country is required")
            .MaximumLength(57)
            .WithMessage("Supplier country cannot exceed 57 characters");
        
        RuleFor(s => s.ContactEmail)
            .NotEmpty()
            .WithMessage("Supplier contact email is required")
            .EmailAddress()
            .WithMessage("Supplier contact email is invalid")
            .MaximumLength(256)
            .WithMessage("Supplier contact email cannot exceed 256 characters");

        RuleFor(s => s.Phone)
            .NotEmpty()
            .WithMessage("Supplier phone is required")
            .MaximumLength(20)
            .WithMessage("Supplier phone cannot exceed 20 characters");
    }
}