using Domain.Models;

namespace IntegrationTests.Builders;

public class WarehouseItemBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _productId = Guid.NewGuid().ToString();
    private const string Location = "Beirut";
    private int _initialQuantity;

    public WarehouseItemBuilder WithProductId(string productId)
    {
        _productId = productId;
        return this;
    }

    public WarehouseItemBuilder ForProduct(Product product)
    {
        return WithProductId(product.Id);
    }

    public WarehouseItemBuilder WithQuantity(int quantity)
    {
        _initialQuantity = quantity;
        return this;
    }

    public WarehouseItem Build()
    {
        var item = new WarehouseItem
        {
            Id = _id,
            ProductId = _productId,
            Location = Location
        };

        // setting quantity on-creation
        if (_initialQuantity > 0)
        {
            item.StockIn(_initialQuantity);
        }

        return item;
    }
}
