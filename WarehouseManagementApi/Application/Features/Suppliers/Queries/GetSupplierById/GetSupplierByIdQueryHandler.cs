using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<GetSupplierByIdQuery, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.Length != 36)
            throw new ArgumentException("Invalid ID format");
        
        var supplier = supplierRepository.GetById(request.Id);
        
        if(supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        return mapper.Map<SupplierViewModel>(supplier);
    }
}