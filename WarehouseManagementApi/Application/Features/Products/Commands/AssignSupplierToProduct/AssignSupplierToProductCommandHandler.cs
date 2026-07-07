using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommandHandler(
    IProductRepository productRepository, ISupplierRepository supplierRepository)
    : IRequestHandler<AssignSupplierToProductCommand, Product>
{
    public async Task<Product> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetById(request.Id);
        
        if(product == null)
            throw new KeyNotFoundException("Product not found");
        
        if(product.IsArchived)
            throw new ArgumentException("Product is unavailable");

        var supplier = supplierRepository.GetById(request.SupplierId);

        if (supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        product.AssignSupplier(supplier);
        return product;
    }
}