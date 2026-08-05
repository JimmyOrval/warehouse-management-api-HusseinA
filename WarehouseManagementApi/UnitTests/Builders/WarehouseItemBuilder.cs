using Domain.Models;

namespace Tests.Builders;

public class WarehouseItemBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();
    private string _productId;
    private string _Location;

    public WarehouseItem Build()
    {
        return new WarehouseItem
        {
            Id = _id,
            ProductId = _productId,
            Location = _Location,
        };
    }
}