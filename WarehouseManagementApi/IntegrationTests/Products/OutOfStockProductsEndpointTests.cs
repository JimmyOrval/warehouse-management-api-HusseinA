using System.Net;
using Application.ViewModels;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Builders;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Products;

[Collection("Warehouse API")]
public class OutOfStockProductsEndpointTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();

    private Domain.Models.Product _zeroQuantityProduct = null!;
    private Domain.Models.Product _inStockProduct = null!;
    private Domain.Models.Product _noWarehouseItemProduct = null!;
    private Domain.Models.Product _archivedZeroQuantityProduct = null!;

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var supplier = new SupplierBuilder().Build();

        _zeroQuantityProduct = new ProductBuilder().WithSku("ZERO-QTY-SKU").WithSupplierId(supplier.Id).Build();
        _inStockProduct = new ProductBuilder().WithSku("IN-STOCK-SKU").WithSupplierId(supplier.Id).Build();
        _noWarehouseItemProduct = new ProductBuilder().WithSku("NO-ITEM-SKU").WithSupplierId(supplier.Id).Build();
        _archivedZeroQuantityProduct = new ProductBuilder()
            .WithSku("ARCHIVED-ZERO-QTY-SKU")
            .WithSupplierId(supplier.Id)
            .Archived()
            .Build();

        // no WithQuantity() call = quantity stays at 0 by default
        var zeroItem = new WarehouseItemBuilder().ForProduct(_zeroQuantityProduct).Build();
        var stockedItem = new WarehouseItemBuilder().ForProduct(_inStockProduct).WithQuantity(50).Build();
        var archivedItem = new WarehouseItemBuilder().ForProduct(_archivedZeroQuantityProduct).Build();
        // _noWarehouseItemProduct deliberately gets no WarehouseItem row at all

        db.Suppliers.Add(supplier);
        db.Products.AddRange(
            _zeroQuantityProduct, _inStockProduct, _noWarehouseItemProduct, _archivedZeroQuantityProduct);
        db.WarehouseItems.AddRange(zeroItem, stockedItem, archivedItem);
        await db.SaveChangesAsync(CancellationToken.None);
    }

    public ValueTask DisposeAsync() => new(Task.CompletedTask);

    [Fact]
    public async Task Returns_Products_With_Zero_Stock_Only()
    {
        var response = await _client.GetAsync("/api/products/out-of-stock", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = (await response.Content.ReadAsAsync<List<ProductViewModel>>());

        products.Should().Contain(p => p.Sku == _zeroQuantityProduct.Sku);
        products.Should().Contain(p => p.Sku == _noWarehouseItemProduct.Sku);
        products.Should().NotContain(p => p.Sku == _inStockProduct.Sku);
        products.Should().NotContain(p => p.Sku == _archivedZeroQuantityProduct.Sku);
    }
}
