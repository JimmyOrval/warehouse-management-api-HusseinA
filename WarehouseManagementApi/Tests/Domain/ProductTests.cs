using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;
using Domain.Models;

namespace Tests.Domain;

public class ProductTests
{
    // tells test runner to run test as data theory
    [Theory]
    // sample input
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void ChangePrice_CannotBe_ZeroOrNegative(decimal price)
    {
        var product = CreateProduct();

        // price cannot become negative
        Assert.Throws<ValidationException>(() =>
            product.ChangePrice(price));
    }
    
    [Fact]
    public void ArchivedProduct_CannotChangePrice()
    {
        var product = CreateProduct();

        product.Archive();

        Assert.Throws<BusinessRuleException>(() =>
            product.ChangePrice(500));
    }
    
    [Fact]
    public void CannotAssignInactiveSupplier()
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Supplier",
            IsActive = false
        };

        var product = CreateProduct();

        Assert.Throws<BusinessRuleException>(() =>
            product.AssignSupplier(supplier));
    }

    private static Product CreateProduct()
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Laptop",
            Sku = "ABC",
            Description = "Test",
            Price = 100,
            SupplierId = Guid.NewGuid().ToString(),
            ExpiryDate = DateTime.Now.AddMonths(6)
        };
    }
}