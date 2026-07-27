using Application.Features.WarehouseItems.Commands.AdjustItemQuantity;
using Application.ViewModels;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using FluentAssertions;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.WarehouseItems;

public class AdjustStockTests
{
    private static (AdjustItemQuantityCommandHandler handler,
        WarehouseItem item) CreateHandler(int initialQuantity)
    {
        var item = new WarehouseItemBuilder().Build();
        item.StockIn(initialQuantity);
        
        var repository = RepositoryMockHelper.MockRepository<IWarehouseItemRepository>();
        repository.Setup(r => r.GetByIdAsync(item.Id, CancellationToken.None))
            .ReturnsAsync(item);
        
        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<WarehouseItem, WarehouseItemViewModel>();
        });

        return (new AdjustItemQuantityCommandHandler(
            repository.Object,
            CommonMocksHelper.MockEventPublisher(),
            CommonMocksHelper.MockConfiguration(),
            mapper,
            CommonMocksHelper.MockLogger<AdjustItemQuantityCommandHandler>()), item);
    }
    
    [Fact]
    public async Task ValidQuantity_UpdatesStock()
    {
        const int initialQuantity = 100;
        
        var (handler, item) = CreateHandler(initialQuantity);

        var result = await handler.Handle(
            new AdjustItemQuantityCommand(
                item.Id, 50), CancellationToken.None);

        result.QuantityInStock.Should().Be(150);
        result.QuantityInStock.Should().BeGreaterThan(initialQuantity);
    }
    
    // my logic allows negative quantity, but I don't allow
    // negative stock, so that's what I will test
    [Fact]
    public async Task Negative_Quantity_Rejected()
    {
        const int initialQuantity = 10;
        
        var (handler, item) = CreateHandler(initialQuantity);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            handler.Handle(new AdjustItemQuantityCommand(
                item.Id, -20), CancellationToken.None));
    }

    [Fact]
    public async Task Adjustment_Updates_LastStockUpdate()
    {
        const int initialQuantity = 10;
        
        var (handler, item) = CreateHandler(initialQuantity);
        var lastUpdated = item.LastStockUpdate;

        await handler.Handle(
            new AdjustItemQuantityCommand(
                item.Id, 50), CancellationToken.None);
        
        Assert.True(item.LastStockUpdate > lastUpdated);
    }
}