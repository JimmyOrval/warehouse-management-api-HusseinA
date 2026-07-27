using Domain.Enums;
using Domain.Models;

namespace Tests.Builders;

public class ProductBuilder
{
    private string _id = Guid.NewGuid().ToString();

    private const string Name = "Product1";

    private string _sku = "PRODUCT-1-SKU";

    private const string Description = "Product 1 Description";

    private decimal _price = 1000.00m;
    
    private string _supplierId = Guid.NewGuid().ToString();
    
    private DateTime _expiryDate = DateTime.UtcNow.AddMonths(1);
    

    public ProductBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithSku(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithSupplierId(string supplierId)
    {
        _supplierId = supplierId;
        return this;
    }

    public ProductBuilder WithExpiryDate(DateTime expiryDate)
    {
        _expiryDate = expiryDate;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = Name,
            Sku = _sku,
            Description = Description,
            Price = _price,
            SupplierId = _supplierId,
            ExpiryDate = _expiryDate
        };
    }
}