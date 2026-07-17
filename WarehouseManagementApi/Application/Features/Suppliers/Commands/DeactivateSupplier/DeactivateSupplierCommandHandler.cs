using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<DeactivateSupplierCommand, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if(supplier == null)
            throw new NotFoundException($"Supplier '{request.Id}' not found");
        
        supplier.Deactivate();
        await supplierRepository.SaveChangesAsync(cancellationToken);
        
        return mapper.Map<SupplierViewModel>(supplier);
    }
}