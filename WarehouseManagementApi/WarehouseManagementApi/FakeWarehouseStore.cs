using WarehouseManagementApi.Models;

namespace WarehouseManagementApi;

public class FakeWarehouseStore
{
    public static List<Product> Products { get; set; }

    static FakeWarehouseStore()
    {
        Products =
        [
            new Product
            {
                Id = "93a46de1-6427-486c-bdca-5faaa11da0d8",
                Name = "Laptop",
                Sku = "LAPTOP-SKU",
                Description = "HP Laptop",
                Price = 1000.00m,
                QuantityInStock = 10,
                SupplierName = "Supplier1",
                ExpiryDate = DateTime.Now.AddMonths(1),
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-1),
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "0ab26dda-2a10-4241-8647-a0b86d56fcf0",
                Name = "Mouse",
                Sku = "MOUSE-SKU",
                Description = "Razer Mouse",
                Price = 100.00m,
                QuantityInStock = 20,
                SupplierName = "Supplier2",
                ExpiryDate = DateTime.Now.AddMonths(2),
                IsArchived = true,
                CreatedAt = DateTime.Now.AddDays(-2),
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "d53c35c2-f668-4191-9631-fcee3de9081d",
                Name = "Keyboard",
                Sku = "KEYBOARD-SKU",
                Description = "HyperX Keyboard",
                Price = 200.00m,
                QuantityInStock = 30,
                SupplierName = "Supplier3",
                ExpiryDate = DateTime.Now.AddMonths(3),
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-3),
                LastUpdatedAt = DateTime.Now
            }
        ];
    }
}