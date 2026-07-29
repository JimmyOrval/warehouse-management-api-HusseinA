using Domain.Exceptions;
using Domain.Models;

namespace Tests.Domain;

public class WarehouseItemTests
{
    // tell test runner to run test as fact with no input
    [Fact]
    public void ProductQuantity_CannotBecomeNegative()
    {
        var item = new WarehouseItem
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = Guid.NewGuid().ToString(),
            Location = "Beirut",
        };
        
        item.StockIn(5);

        // to check if it accepts update to negative value
        // StockOut() subtracts the input
        Assert.Throws<BusinessRuleException>(() =>
            item.StockOut(10));
    }
}