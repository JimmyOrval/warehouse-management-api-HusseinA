using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.AssignSupplierToProduct;

public class AssignSupplierToProductCommandHandler(
    IProductRepository productRepository, ISupplierRepository supplierRepository)
    : IRequestHandler<AssignSupplierToProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
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
        return new ProductDto(
            product.Id, product.Name,  product.Sku, product.Description, product.Price,
            product.SupplierId, product.ExpiryDate, product.IsArchived,
            product.CreatedAt, product.LastUpdatedAt);
    }
}