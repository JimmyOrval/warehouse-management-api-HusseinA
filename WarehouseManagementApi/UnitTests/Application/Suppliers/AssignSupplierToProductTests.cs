using Application.Features.Suppliers.Commands.AssignSupplierToProduct;
using Application.ViewModels;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Suppliers;

public class AssignSupplierToProductTests
{
    [Fact]
    public async Task Valid_AssignSupplierToProduct_Succeeds()
    {
        var supplier = new SupplierBuilder().Build();
        var product = new ProductBuilder().Build();
        
        var productRepository = RepositoryMockHelper.MockRepository<IProductRepository>();
        productRepository.Setup(r => r.GetByIdAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        
        var supplierRepository = RepositoryMockHelper.MockRepository<ISupplierRepository>();
        supplierRepository.Setup(r => r.GetByIdAsync(supplier.Id, CancellationToken.None))
            .ReturnsAsync(supplier);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Supplier, SupplierViewModel>();
            cfg.CreateMap<Product, ProductViewModel>();
        });
        
        var handler = new AssignSupplierToProductCommandHandler(
            productRepository.Object,
            supplierRepository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<AssignSupplierToProductCommandHandler>());
        
        await handler.Handle(
            new AssignSupplierToProductCommand(
                product.Id, supplier.Id),
            CancellationToken.None);
        
        Assert.Equal(supplier.Id, product.SupplierId);
    }
    
    [Fact]
    public async Task Assigning_ArchivedProduct_Fails()
    {
        var supplier = new SupplierBuilder().Build();
        var product = new ProductBuilder().Build();
        product.Archive();
        
        var productRepository = RepositoryMockHelper.MockRepository<IProductRepository>();
        productRepository.Setup(r => r.GetByIdAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        
        var supplierRepository = RepositoryMockHelper.MockRepository<ISupplierRepository>();
        supplierRepository.Setup(r => r.GetByIdAsync(supplier.Id, CancellationToken.None))
            .ReturnsAsync(supplier);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Supplier, SupplierViewModel>();
            cfg.CreateMap<Product, ProductViewModel>();
        });
        
        var handler = new AssignSupplierToProductCommandHandler(
            productRepository.Object,
            supplierRepository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<AssignSupplierToProductCommandHandler>());

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
        
            handler.Handle(
                new AssignSupplierToProductCommand(
                    product.Id, supplier.Id),
                CancellationToken.None)
        );
    }
    
    [Fact]
    public async Task Assigning_MissingSupplier_Fails()
    {
        var product = new ProductBuilder().Build();
        
        var productRepository = RepositoryMockHelper.MockRepository<IProductRepository>();
        productRepository.Setup(r => r.GetByIdAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Supplier, SupplierViewModel>();
            cfg.CreateMap<Product, ProductViewModel>();
        });
        
        var handler = new AssignSupplierToProductCommandHandler(
            productRepository.Object,
            RepositoryMockHelper.MockRepository<ISupplierRepository>().Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<AssignSupplierToProductCommandHandler>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
        
            handler.Handle(
                new AssignSupplierToProductCommand(
                    product.Id, Guid.NewGuid().ToString()),
                CancellationToken.None)
        );
    }
}