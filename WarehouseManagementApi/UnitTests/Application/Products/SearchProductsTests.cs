using Application.Features.Products.Queries.SearchProducts;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using FluentAssertions;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class SearchProductsTests
{
    private static SearchProductsQueryHandler CreateHandler(List<Product> products)
    {
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();

        repository.Setup(r => r.SearchAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        return new SearchProductsQueryHandler(repository.Object,
            CommonMocksHelper.MockMapper(cfg =>
            {
                cfg.CreateMap<Product, ProductViewModel>();
            }),
            CommonMocksHelper.MockLogger<SearchProductsQueryHandler>());
    }

    [Fact]
    public async Task Search_ByName_ReturnsMatches()
    {
        var product1 = new ProductBuilder().WithName("Laptop 1").Build();
        var product2 = new ProductBuilder().WithName("Laptop 2").Build();
        
        var handler = CreateHandler([product1, product2]);
        
        var result = await handler.Handle(new SearchProductsQuery("Laptop", null), CancellationToken.None);
        var products = result.ToList();
        
        products.Should().HaveCount(2);
        products.Should().Contain(x => x.Name == "Laptop 1");
        products.Should().Contain(x => x.Name == "Laptop 2");
    }

    [Fact]
    public async Task Search_BySupplier_ReturnsMatches()
    {
        var product1 = new ProductBuilder().WithSupplier(Guid.NewGuid().ToString() , "Supplier 1").Build();
        var product2 = new ProductBuilder().WithSupplier(Guid.NewGuid().ToString() , "Supplier 2").Build();
        
        var handler = CreateHandler([product1, product2]);
        
        var result = await handler.Handle(new SearchProductsQuery(null, "Supplier"), CancellationToken.None);
        var products = result.ToList();
        
        products.Should().HaveCount(2);
        products.Should().Contain(x => x.SupplierName == "Supplier 1");
        products.Should().Contain(x => x.SupplierName == "Supplier 2");
    }

    [Fact]
    public async Task Search_ByBothFilters_ReturnsIntersection()
    {
        var product1 = new ProductBuilder().WithSupplier(Guid.NewGuid().ToString(),
            "Supplier 1").Build();
        var product2 = new ProductBuilder().WithName("Laptop 2").Build();
        
        var handler = CreateHandler([product1, product2]);
        
        var result = await handler.Handle(
            new SearchProductsQuery("Laptop", "Supplier"),
            CancellationToken.None);
        var products = result.ToList();
        
        products.Should().HaveCount(2);
        products.Should().Contain(x => x.SupplierName == "Supplier 1");
        products.Should().Contain(x => x.Name == "Laptop 2");
    }
    
    [Fact]
    public async Task EmptyFilters_Return_BadRequestException()
    { 
        var query = new SearchProductsQuery(null, null);
        var validator = new SearchProductsQueryValidator();
        
        var validationResult = await validator.ValidateAsync(query, CancellationToken.None);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors,
            e => e.ErrorMessage ==
                 "Both filters empty. Please enter at least one.");
    }
}