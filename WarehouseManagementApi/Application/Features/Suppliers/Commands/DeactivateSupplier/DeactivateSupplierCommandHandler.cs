using Application.DTOs;
using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Commands.DeactivateSupplier;

public class DeactivateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<DeactivateSupplierCommand, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var supplier = supplierRepository.GetById(request.Id);
        
        if(supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        supplier.IsActive = false;
        return mapper.Map<SupplierViewModel>(supplier);
    }
}