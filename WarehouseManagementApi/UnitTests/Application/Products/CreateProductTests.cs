using Application.Features.Products.Commands.CreateProduct;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class CreateProductTests
{
    [Fact]
    public async Task Create_ValidProduct_Succeeds()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        await HandleProductCreation(repository);
        repository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Created_Date_Assigned()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        var id = await HandleProductCreation(repository);
        Assert.NotNull(id);
        repository.Verify(r => r.Add(It.Is<Product>(
            p => p.CreatedAt != default)));
    }

    [Fact]
    public async Task GeneratedId_IsNotEmpty()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        var id = await HandleProductCreation(repository);
        Assert.NotEmpty(id);
    }

    [Fact]
    public async Task Duplicate_Sku_ThrowsBusinessRuleException()
    {
        var product = new ProductBuilder().WithSku("SKU").Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        // set up repository as if a product already exists with this SKU
        repository
            .Setup(r => r.SkuExistsAsync("SKU", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); 
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<CreateProductCommand, Product>();
        });
        
        var handler = new CreateProductCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockEventPublisher(),
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<CreateProductCommandHandler>());
        
        var command = new CreateProductCommand(
            product.Name,
            product.Sku,
            product.Description,
            product.Price,
            product.SupplierId,
            product.ExpiryDate);

        // makes sure it throws exception when our product is created
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    private static async Task<string> HandleProductCreation(Mock<IProductRepository> repository)
    {
        var product = new ProductBuilder().Build();
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<CreateProductCommand, Product>();
        });
        
        var handler = new CreateProductCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockEventPublisher(),
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<CreateProductCommandHandler>());
        
        var command = new CreateProductCommand(
            product.Name,
            product.Sku,
            product.Description,
            product.Price,
            product.SupplierId,
            product.ExpiryDate);
        
        return await handler.Handle(command, CancellationToken.None);
    }
}