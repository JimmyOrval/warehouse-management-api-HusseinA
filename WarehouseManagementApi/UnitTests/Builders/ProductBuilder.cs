using Domain.Enums;
using Domain.Models;

namespace Tests.Builders;

public class ProductBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();

    private string _name = "Product1";

    private string _sku = "PRODUCT-1-SKU";

    private const string Description = "Product 1 Description";

    private decimal _price = 1000.00m;
    
    private string _supplierId = Guid.NewGuid().ToString();
    
    private readonly DateTime _expiryDate = DateTime.UtcNow.AddMonths(1);
    
    private string _supplierName = "Supplier";
    

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

    public ProductBuilder WithSupplier(string supplierId, string supplierName)
    {
        _supplierId = supplierId;
        _supplierName = supplierName;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = _name,
            Sku = _sku,
            Description = Description,
            Price = _price,
            SupplierId = _supplierId,
            ExpiryDate = _expiryDate,
            Supplier = new Supplier
            {
                Id = _supplierId,
                Name = _supplierName
            }
        };
    }
}