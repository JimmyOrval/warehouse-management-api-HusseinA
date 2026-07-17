using FluentValidation;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommandValidator
    : AbstractValidator<DeactivateSupplierCommand>
{
    public DeactivateSupplierCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Supplier ID is required")
            .Length(36)
            .WithMessage("Supplier ID format invalid");
    }
}