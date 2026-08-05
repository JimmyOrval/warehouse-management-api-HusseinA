using System.Net;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Builders;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Products;

[Collection("Warehouse API")]
public class ProductImageEndpointTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();

    private Domain.Models.Supplier _seededSupplier = null!;
    private Domain.Models.Product _seededProduct = null!;

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        _seededSupplier = new SupplierBuilder().Build();
        _seededProduct = new ProductBuilder().WithSupplierId(_seededSupplier.Id).Build();

        db.Suppliers.Add(_seededSupplier);
        db.Products.Add(_seededProduct);
        await db.SaveChangesAsync(CancellationToken.None);
    }

    public ValueTask DisposeAsync() => new(Task.CompletedTask);

    [Fact]
    public async Task UploadImage_WithValidJpg_Succeeds()
    {
        var content = MultipartFormHelper.JpgImage(fieldName: "image");

        var response = await _client.PostAsync(
            $"/api/products/{_seededProduct.Id}/image", content, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var imageId = await response.Content.ReadAsAsync<string>();
        imageId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UploadImage_WithValidPng_Succeeds()
    {
        using var content = MultipartFormHelper.PngImage(fieldName: "image");

        var response = await _client.PostAsync(
            $"/api/products/{_seededProduct.Id}/image", content, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var imageId = await response.Content.ReadAsAsync<string>();
        imageId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UploadImage_WithTxtFile_Fails()
    {
        using var content = MultipartFormHelper.InvalidExtensionFile(fieldName: "image");

        var response = await _client.PostAsync(
            $"/api/products/{_seededProduct.Id}/image", content, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadImage_ExceedingSizeLimit_Fails()
    {
        using var content = MultipartFormHelper.OversizedImage(
            "test.jpg", "image/jpg");

        var response = await _client.PostAsync(
            $"/api/products/{_seededProduct.Id}/image", content, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadImage_ToMissingProduct_Returns404()
    {
        using var content = MultipartFormHelper.JpgImage(fieldName: "image");

        var response = await _client.PostAsync(
            $"/api/products/{Guid.NewGuid()}/image", content, CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
