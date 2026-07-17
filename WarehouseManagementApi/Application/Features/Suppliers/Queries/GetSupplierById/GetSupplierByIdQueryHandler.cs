using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<GetSupplierByIdQuery, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        return supplier == null
            ? throw new NotFoundException($"Supplier '{request.Id}' not found")
            : mapper.Map<SupplierViewModel>(supplier);
    }
}