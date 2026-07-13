using Application.ViewModels;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommandHandler(
    IProductRepository productRepository,
    ISupplierRepository supplierRepository,
    IMapper mapper)
    : IRequestHandler<AssignSupplierToProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if(product == null)
            throw new KeyNotFoundException("Product not found");
        
        if(product.IsArchived)
            throw new ArgumentException("Product is unavailable");

        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        product.AssignSupplier(supplier);
        await productRepository.SaveChangesAsync(cancellationToken);
        
        return mapper.Map<ProductViewModel>(product);
    }
}