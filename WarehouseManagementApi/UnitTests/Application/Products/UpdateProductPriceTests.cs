using Application.Features.Products.Commands.UpdateProductPrice;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using FluentAssertions;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class UpdateProductPriceTests
{
    private static
        (UpdateProductPriceCommandHandler handler, 
        Product product)
        CreateHandler()
    {
        var product = new ProductBuilder().WithPrice(1000).Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });

        return (new UpdateProductPriceCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<UpdateProductPriceCommandHandler>()),
            product);
    }
    
    [Fact]
    public async Task ValidPrice_Updates()
    {
        var (handler, product) = CreateHandler();
        
        await handler.Handle(
            new UpdateProductPriceCommand(
                product.Id, 1500),
            CancellationToken.None);
        
        product.Price.Should().Be(1500);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task InvalidPrice_Rejected(int invalidPrice)
    {
        var (handler, product) = CreateHandler();
        var query = new UpdateProductPriceCommand(product.Id, invalidPrice);
        
        var validator = new UpdateProductPriceCommandValidator();
        
        var validationResult = await validator.ValidateAsync(query,
            CancellationToken.None);
        
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e =>
            e.ErrorMessage == 
            "New Price cannot be negative");
    }
}