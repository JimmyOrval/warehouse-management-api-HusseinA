using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(
    ISupplierRepository supplierRepository,
    IMapper mapper,
    ILogger<GetSupplierByIdQueryHandler> logger)
    : IRequestHandler<GetSupplierByIdQuery, SupplierViewModel>
{
    public async Task<SupplierViewModel> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier == null)
        {
            logger.LogInformation("Supplier {SupplierId} not found", request.Id);
            throw new NotFoundException($"Supplier '{request.Id}' not found");
        }
        
        logger.LogInformation("Supplier {SupplierId} retrieved", supplier.Id);
        
        return mapper.Map<SupplierViewModel>(supplier);
    }
}