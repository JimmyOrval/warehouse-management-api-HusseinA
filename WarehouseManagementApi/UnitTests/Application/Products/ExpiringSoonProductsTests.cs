using Application.Features.Products.Queries.ExpiringSoonProducts;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class ExpiringSoonProductsTests
{
    [Fact]
    public async Task Handle_Calls_Repository_With_Utc_Now_And_30_Day_Cutoff()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository
            .Setup(r => r.GetExpiringSoonAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
 
        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });
 
        var handler = new ExpiringSoonProductsQueryHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockLogger<ExpiringSoonProductsQueryHandler>());
 
        var before = DateTime.UtcNow;
        await handler.Handle(new ExpiringSoonProductsQuery(), CancellationToken.None);
        var after = DateTime.UtcNow;
 
        repository.Verify(r => r.GetExpiringSoonAsync(
            It.Is<DateTime>(from => from >= before && from <= after),
            It.Is<DateTime>(to => to >= before.AddDays(30) && to <= after.AddDays(30)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
 
    [Fact]
    public async Task Handle_Returns_Empty_When_Repository_Returns_Empty()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository
            .Setup(r => r.GetExpiringSoonAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
 
        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });
 
        var handler = new ExpiringSoonProductsQueryHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockLogger<ExpiringSoonProductsQueryHandler>());
 
        var result = await handler.Handle(new ExpiringSoonProductsQuery(), CancellationToken.None);
 
        Assert.Empty(result);
    }
 
    [Fact]
    public async Task Handle_Maps_Repository_Results_To_ViewModels()
    {
        var product = new ProductBuilder()
            .WithSku("EXPIRING-SOON-SKU")
            .Build();
 
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository
            .Setup(r => r.GetExpiringSoonAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);
 
        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });
 
        var handler = new ExpiringSoonProductsQueryHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockLogger<ExpiringSoonProductsQueryHandler>());
 
        var result = (await handler.Handle(new ExpiringSoonProductsQuery(), CancellationToken.None)).ToList();
 
        Assert.Single(result);
        Assert.Equal(product.Sku, result[0].Sku);
    }
}