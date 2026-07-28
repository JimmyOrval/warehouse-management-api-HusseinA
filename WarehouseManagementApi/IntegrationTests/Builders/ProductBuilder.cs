using Domain.Models;

namespace IntegrationTests.Builders;

public class ProductBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();
    private string _name = "Product1";
    private string _sku = "PRODUCT-1-SKU";
    private const string Description = "Product 1 Description";
    private decimal _price = 1000.00m;
    private string _supplierId = Guid.NewGuid().ToString();
    private readonly DateTime _expiryDate = DateTime.UtcNow.AddMonths(1);
    private Supplier? _supplier;

    // to set it as archived on-creation (explained in Build() below)
    private bool _archived;

    public ProductBuilder WithName(string name)
    {
        _name = name;
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

    public ProductBuilder Archived()
    {
        _archived = true;
        return this;
    }

    public Product Build()
    {
        var product = new Product
        {
            Id = _id,
            Name = _name,
            Sku = _sku,
            Description = Description,
            Price = _price,
            SupplierId = _supplierId,
            ExpiryDate = _expiryDate,
            Supplier = _supplier
        };

        // since status is privately set, I created
        // an automated way to archive it on-creating
        if (_archived)
        {
            product.Archive();
        }

        return product;
    }
}
