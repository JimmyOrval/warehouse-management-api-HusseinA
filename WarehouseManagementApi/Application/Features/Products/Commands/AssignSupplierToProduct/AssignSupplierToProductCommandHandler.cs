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
        var product = await productRepository.GetById(request.Id);
        
        if(product == null)
            throw new KeyNotFoundException("Product not found");
        
        if(product.IsArchived)
            throw new ArgumentException("Product is unavailable");

        var supplier = supplierRepository.GetById(request.SupplierId);

        if (supplier == null)
            throw new KeyNotFoundException("Supplier not found");
        
        product.AssignSupplier(supplier);
        await productRepository.SaveChangesAsync();
        
        return mapper.Map<ProductViewModel>(product);
    }
}