using System.Net;
using Application.Common;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.ViewModels;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Builders;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Suppliers;

[Collection("Warehouse API")]
public class SupplierEndpointTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();

    private Domain.Models.Supplier _seededSupplier = null!;
    private Domain.Models.Supplier _seededInactiveSupplier = null!;
    private Domain.Models.Product _seededProduct = null!;

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var cacheStats = scope.ServiceProvider.GetRequiredService<ICacheStatsTracker>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        
        foreach (var key in cacheStats.GetStats().CachedKeys)
        {
            await cache.RemoveAsync(key, CancellationToken.None);
        }

        _seededSupplier = new SupplierBuilder().WithName("Acme Supplies").Build();
        _seededInactiveSupplier = new SupplierBuilder()
            .WithName("Defunct Co")
            .Inactive()
            .Build();

        _seededProduct = new ProductBuilder()
            .WithSku("SUPPLIER-TEST-SKU")
            .WithSupplierId(_seededSupplier.Id)
            .Build();

        db.Suppliers.AddRange(_seededSupplier, _seededInactiveSupplier);
        db.Products.Add(_seededProduct);
        await db.SaveChangesAsync(CancellationToken.None);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }

    [Fact]
    public async Task CreateSupplier_Returns201()
    {
        var command = new CreateSupplierCommand(
            Name: "Supplier1",
            Country: "Lebanon",
            ContactEmail: "supplier1@email.com",
            Phone: "+9611234567");

        var response = await _client.PostJsonAsync("/api/suppliers", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        
        var getResponse = await _client.GetAsync(response.Headers.Location, CancellationToken.None);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await getResponse.Content.ReadAsAsync<SupplierViewModel>();
        created.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task GetSupplierById_ReturnsSupplier()
    {
        var response = await _client.GetAsync($"/api/suppliers/{_seededSupplier.Id}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var supplier = await response.Content.ReadAsAsync<SupplierViewModel>();
        supplier.Name.Should().Be(_seededSupplier.Name);
        supplier.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateSupplier_ReturnsUpdatedSupplier()
    {
        var response = await _client.DeleteAsync($"/api/suppliers/{_seededSupplier.Id}", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var supplier = await response.Content.ReadAsAsync<SupplierViewModel>();
        supplier.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivatedSupplier_StaysAsInactive()
    {
        await _client.DeleteAsync($"/api/suppliers/{_seededSupplier.Id}", CancellationToken.None);

        var getResponse = await _client.GetAsync($"/api/suppliers/{_seededSupplier.Id}", CancellationToken.None);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var supplier = await getResponse.Content.ReadAsAsync<SupplierViewModel>();
        supplier.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task AssignSupplierToProduct_UpdatesProduct()
    {
        var newSupplier = new SupplierBuilder().WithName("Replacement Supplier").Build();
        // temporary db for product
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
            db.Suppliers.Add(newSupplier);
            await db.SaveChangesAsync(CancellationToken.None);
        }

        var response = await _client.PostAsync(
            $"/api/suppliers/{_seededProduct.Id}/assign-supplier/{newSupplier.Id}", null, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadAsAsync<ProductViewModel>();
        product.SupplierId.Should().Be(newSupplier.Id);
    }

    [Fact]
    public async Task AssignInactiveSupplier_IsRejected()
    {
        var response = await _client.PostAsync(
            $"/api/suppliers/{_seededProduct.Id}/assign-supplier/{_seededInactiveSupplier.Id}",
            null, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AssignMissingSupplier_Returns404()
    {
        var response = await _client.PostAsync(
            $"/api/suppliers/{_seededProduct.Id}/assign-supplier/{Guid.NewGuid()}",
            null, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
