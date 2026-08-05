using System.Net;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Builders;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.WarehouseItems;

[Collection("Warehouse API")]
public class WarehouseItemEndpointTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();

    private Domain.Models.Supplier _seededSupplier = null!;
    private Domain.Models.Product _seededProduct = null!;
    private Domain.Models.WarehouseItem _seededItem = null!;
    // item needs product to be created and product needs supplier

    private const int SeededQuantity = 50;

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        _seededSupplier = new SupplierBuilder().Build();
        _seededProduct = new ProductBuilder().WithSupplierId(_seededSupplier.Id).Build();
        _seededItem = new WarehouseItemBuilder()
            .ForProduct(_seededProduct)
            .WithQuantity(SeededQuantity)
            .Build();

        db.Suppliers.Add(_seededSupplier);
        db.Products.Add(_seededProduct);
        db.WarehouseItems.Add(_seededItem);
        await db.SaveChangesAsync();
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }

    private async Task<int> GetPersistedQuantityAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        var item = await db.WarehouseItems.AsNoTracking()
            .SingleAsync(i => i.Id == _seededItem.Id);
        return item.QuantityInStock;
    }

    [Fact]
    public async Task AdjustQuantity_WithPositiveValue_IncreasesStock()
    {
        var response = await _client.PutAsync(
            $"/api/warehouse-items/{_seededItem.Id}/quantity?quantity=10",
            null,
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetPersistedQuantityAsync()).Should().Be(SeededQuantity + 10);
    }

    [Fact]
    public async Task AdjustQuantity_WithNegativeValue_DecreasesStock()
    {
        var response = await _client.PutAsync(
            $"/api/warehouse-items/{_seededItem.Id}/quantity?quantity=-10",
            null,
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetPersistedQuantityAsync()).Should().Be(SeededQuantity - 10);
    }

    [Fact]
    public async Task AdjustQuantity_WithZero_Fails()
    {
        var response = await _client.PutAsync(
            $"/api/warehouse-items/{_seededItem.Id}/quantity?quantity=0",
            null,
            CancellationToken.None);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await GetPersistedQuantityAsync()).Should().Be(SeededQuantity);
    }

    [Fact]
    public async Task RemoveQuantity_ExceedingAvailableStock_Fails()
    {
        var response = await _client.PutAsync(
            $"/api/warehouse-items/{_seededItem.Id}/quantity?quantity=-{SeededQuantity + 1}",
            null,
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await GetPersistedQuantityAsync()).Should().Be(SeededQuantity);
    }

    [Fact]
    public async Task AdjustQuantity_OnMissingItem_Returns404()
    {
        var response = await _client.PutAsync(
            $"/api/warehouse-items/{Guid.NewGuid()}/quantity?quantity=5", null,
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
