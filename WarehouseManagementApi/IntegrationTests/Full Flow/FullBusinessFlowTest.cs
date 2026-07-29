using System.Net;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Features.WarehouseItems.Commands.CreateWarehouseItem;
using Application.ViewModels;
using Domain.Enums;
using FluentAssertions;
using Infrastructure;
using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Full_Flow;

[Collection("Warehouse API")]
public class FullBusinessFlowTest(CustomWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient().AsAdmin();
    
    private record CreatedIdResponse(string Id);

    [Fact]
    public async Task FullProductLifecycle_CreateAssignUploadAdjustPriceArchive_Succeeds()
    {
        // first create supplier
        var createSupplierCommand = new CreateSupplierCommand(
            Name: "Supplier1",
            Country: "Lebanon",
            ContactEmail: "supplier@email.com",
            Phone: "+9611234567");

        var supplierResponse = await _client.PostJsonAsync("/api/suppliers", createSupplierCommand);
        supplierResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var supplierId = ExtractIdFromLocation(supplierResponse);

        // then create product
        var createProductCommand = new CreateProductCommand(
            Name: "Product1",
            Sku: "PRODUCT-SKU-1",
            Description: "Full flow product",
            Price: 100.00m,
            SupplierId: supplierId,
            ExpiryDate: DateTime.UtcNow.AddMonths(6));

        var productResponse = await _client.PostJsonAsync("/api/products", createProductCommand);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var productId = ExtractIdFromLocation(productResponse);

        // create a second supplier to assign to product
        var secondSupplierCommand = new CreateSupplierCommand(
            Name: "Flow Supplier 2",
            Country: "Lebanon",
            ContactEmail: "flow-supplier-2@email.com",
            Phone: "+9611234568");

        var secondSupplierResponse = await _client.PostJsonAsync("/api/suppliers", secondSupplierCommand);
        secondSupplierResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var secondSupplierId = ExtractIdFromLocation(secondSupplierResponse);

        // assign second supplir to produc
        var assignResponse = await _client.PostAsync(
            $"/api/suppliers/{productId}/assign-supplier/{secondSupplierId}", null, CancellationToken.None);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var assignedProduct = await assignResponse.Content.ReadAsAsync<ProductViewModel>();
        assignedProduct.SupplierId.Should().Be(secondSupplierId);

        // then upload image
        using var imageContent = MultipartFormHelper.JpgImage();

        var uploadResponse = await _client.PostAsync(
            $"/api/products/{productId}/image", imageContent, CancellationToken.None);
        uploadResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var imageId = await uploadResponse.Content.ReadAsAsync<string>();
        imageId.Should().NotBeNullOrWhiteSpace();

        // warehouse item is responsible for quantity, so create it to adjust stock
        var createItemCommand = new CreateWarehouseItemCommand(productId, "Beirut");
        var createItemResponse = await _client.PostJsonAsync("/api/warehouse-items", createItemCommand);
        createItemResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var itemId = (await createItemResponse.Content.ReadAsAsync<CreatedIdResponse>()).Id;

        // then adjust quantity
        var adjustQuantityResponse = await _client.PutAsync(
            $"/api/warehouse-items/{itemId}/quantity?quantity=25", null, CancellationToken.None);
        adjustQuantityResponse.StatusCode.Should().Be(HttpStatusCode.OK);


        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
            var item = await db.WarehouseItems.AsNoTracking()
                .SingleAsync(i => i.Id == itemId,
                CancellationToken.None);
            item.QuantityInStock.Should().Be(25);
        }

        // update product price
        var priceResponse = await _client.PutJsonAsync($"/api/products/{productId}/price", 250);
        priceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // archive product
        var archiveResponse = await _client.DeleteAsync($"/api/products/{productId}", CancellationToken.None);
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // finally check useful values
        var finalResponse = await _client.GetAsync($"/api/products/{productId}", CancellationToken.None);
        finalResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var finalProduct = await finalResponse.Content.ReadAsAsync<ProductViewModel>();

        finalProduct.Status.Should().Be(ProductStatus.Archived);
        finalProduct.Price.Should().Be(250);
        finalProduct.SupplierId.Should().Be(secondSupplierId);
    }

    private static string ExtractIdFromLocation(HttpResponseMessage response)
    {
        var location = response.Headers.Location!.ToString();
        return location[(location.LastIndexOf('/') + 1)..];
    }
}
