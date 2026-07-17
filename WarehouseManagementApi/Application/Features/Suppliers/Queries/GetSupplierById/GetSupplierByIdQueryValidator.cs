using FluentValidation;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryValidator
    : AbstractValidator<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .WithMessage("Supplier ID is required")
            .Length(36)
            .WithMessage("Supplier ID format invalid");
    }
}