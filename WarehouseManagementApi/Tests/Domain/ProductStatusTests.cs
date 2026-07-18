using Domain.Models;
using Domain.Exceptions;

namespace Tests.Domain;

public class ProductStatusTests
{
    [Fact]
    public void Archive_WhenAlreadyArchived_ThrowsInvalidOperationException()
    {
        var product = CreateTestProduct();
        product.Archive();

        var act = product.Archive;

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ChangePrice_WhenArchived_ThrowsBusinessRuleException()
    {
        var product = CreateTestProduct();
        product.Archive();

        Assert.Throws<BusinessRuleException>(((Action?)Act)!);
        return;

        void Act() => product.ChangePrice(50m);
    }

    private static Product CreateTestProduct()
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Product",
            Sku = "SKU-001",
            Description = "Test",
            Price = 10m,
            SupplierId = Guid.NewGuid().ToString(),
            ExpiryDate = DateTime.UtcNow.AddDays(30),
        };
    }
}