using System.Net;
using Application.Common;
using Application.Features.Products.Commands.CreateProduct;
using Application.ViewModels;
using Domain.Enums;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Builders;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Products;

[Collection("Warehouse API")]
public class ProductEndpointTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    // this will act as the mediator
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();
 
    // given values in InitializeAsync instead of filling them each test.
    private Domain.Models.Supplier _seededSupplier = null!;
    private Domain.Models.Product _seededProduct = null!;
    private Domain.Models.Product _seededArchivedProduct = null!;
    private Domain.Models.WarehouseItem _seededWarehouseItem = null!;

    // initialized db and seeds initial data
    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var cacheStats = scope.ServiceProvider.GetRequiredService<ICacheStatsTracker>();
        
        // since memory is persistent while tests are running, we have to
        // recreate the db's contents each time
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        
        // also reset cache keys for every test
        foreach (var key in cacheStats.GetStats().CachedKeys)
        {
            await cache.RemoveAsync(key, CancellationToken.None);
        }
 
        _seededSupplier = new SupplierBuilder().WithName("Supplier1").Build();
        await db.SaveChangesAsync(CancellationToken.None);
 
        _seededProduct = new ProductBuilder()
            .WithSku("LAPTOP-SKU")
            .WithName("Laptop1")
            .WithSupplierId(_seededSupplier.Id)
            .Build();
 
        _seededArchivedProduct = new ProductBuilder()
            .WithSku("ARCHIVED_LAPTOP-SKU")
            .WithName("Archived Laptop")
            .WithSupplierId(_seededSupplier.Id)
            .Archived()
            .Build();
        
        // I need this to check for product availability when getting all
        _seededWarehouseItem = new WarehouseItemBuilder()
            .ForProduct(_seededProduct)
            .WithQuantity(50)
            .Build();
 
        db.Suppliers.Add(_seededSupplier);
        db.Products.AddRange(_seededProduct, _seededArchivedProduct);
        db.WarehouseItems.Add(_seededWarehouseItem);
        await db.SaveChangesAsync(CancellationToken.None);
    }

    // when a test ends, it closes it and its resources
    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }
    
 
    [Fact]
    public async Task GetProducts_ReturnSeededProducts()
    {
        var response = await _client.GetAsync("/api/products", CancellationToken.None);
 
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadAsAsync<List<ProductViewModel>>();
        products.Should().Contain(p => p.Sku == _seededProduct.Sku);
    }
 
    [Fact]
    public async Task GetProducts_OnlyAvailableDefault_ExcludesArchived()
    {
        // onlyAvailable's default is true, so this should not return archived product
        var response = await _client.GetAsync("/api/products", CancellationToken.None);
 
        var products = await response.Content.ReadAsAsync<List<ProductViewModel>>();
        // since SKU is unique
        products.Should().NotContain(p => p.Sku == _seededArchivedProduct.Sku);
    }
 
    [Fact]
    public async Task GetProductById_ReturnsProduct()
    {
        var response = await _client.GetAsync($"/api/products/{_seededProduct.Id}", CancellationToken.None);
 
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadAsAsync<ProductViewModel>();
        product.Sku.Should().Be(_seededProduct.Sku);
        product.SupplierId.Should().Be(_seededSupplier.Id);
    }
 
    [Fact]
    public async Task GetProduct_ByInvalidId_Returns404()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}", CancellationToken.None);
 
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
 
    [Fact]
    public async Task CreateProduct_Returns201()
    {
        var command = new CreateProductCommand(
            Name: "New Product",
            Sku: "NEW-SKU-001",
            Description: "New product description",
            Price: 25.00m,
            SupplierId: _seededSupplier.Id,
            ExpiryDate: DateTime.UtcNow.AddMonths(6));
 
        var response = await _client.PostJsonAsync("/api/products", command);
 
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
 
        // this is to check if product has really been created
        var getResponse = await _client.GetAsync(response.Headers.Location, CancellationToken.None);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await getResponse.Content.ReadAsAsync<ProductViewModel>();
        created.Sku.Should().Be(command.Sku);
    }
 
    [Fact]
    public async Task CreateProduct_WithDuplicateSku_Returns_409()
    {
        var command = new CreateProductCommand(
            Name: "Duplicate SKU Product",
            // same as sku we already have
            Sku: _seededProduct.Sku,
            Description: "Should be rejected",
            Price: 10.00m,
            SupplierId: _seededSupplier.Id,
            ExpiryDate: DateTime.UtcNow.AddMonths(6));
 
        var response = await _client.PostJsonAsync("/api/products", command);
        
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
 
    [Fact]
    public async Task UpdatePrice_WithValidValue_UpdatesProduct()
    {
        const decimal newPrice = 1500.00m;
 
        var response = await _client.PutJsonAsync($"/api/products/{_seededProduct.Id}/price", newPrice);
 
        response.StatusCode.Should().Be(HttpStatusCode.OK);
 
        var getResponse = await _client.GetAsync($"/api/products/{_seededProduct.Id}", CancellationToken.None);
        var product = await getResponse.Content.ReadAsAsync<ProductViewModel>();
        product.Price.Should().Be(newPrice);
    }
 
    [Fact]
    public async Task UpdatePrice_OnArchivedProduct_Fails()
    {
        var response = await _client.PutJsonAsync(
            $"/api/products/{_seededArchivedProduct.Id}/price", 999.00m);
 
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
 
    [Fact]
    public async Task Delete_Archives_Product()
    {
        var response = await _client.DeleteAsync($"/api/products/{_seededProduct.Id}", CancellationToken.None);
 
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
 
    [Fact]
    public async Task DeletedProduct_StillExists_ButArchived()
    {
        await _client.DeleteAsync($"/api/products/{_seededProduct.Id}", CancellationToken.None);
 
        var getResponse = await _client.GetAsync($"/api/products/{_seededProduct.Id}", CancellationToken.None);
 
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await getResponse.Content.ReadAsAsync<ProductViewModel>();
        product.Status.Should().Be(ProductStatus.Archived);
    }
}