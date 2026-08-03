using Application.Features.Products.Queries.GetOutOfStockProducts;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class OutOfStockProductsTests
{
    // Same scope note as ExpiringSoonProductsTests: the actual zero-stock filtering
    // logic lives in ProductRepository's EF query, not here. This only verifies the
    // handler's own job - calling the repository and mapping the result.

    [Fact]
    public async Task Handle_Returns_Empty_When_Nothing_Out_Of_Stock()
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository
            .Setup(r => r.GetOutOfStockAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });

        var handler = new GetOutOfStockProductsQueryHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockLogger<GetOutOfStockProductsQueryHandler>());

        var result = await handler.Handle(new GetOutOfStockProductsQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_Maps_Repository_Results_To_ViewModels()
    {
        var product = new ProductBuilder().WithSku("OUT-OF-STOCK-SKU").Build();

        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository
            .Setup(r => r.GetOutOfStockAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);

        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });

        var handler = new GetOutOfStockProductsQueryHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockLogger<GetOutOfStockProductsQueryHandler>());

        var result = (await handler.Handle(new GetOutOfStockProductsQuery(), CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal(product.Sku, result[0].Sku);
    }
}
