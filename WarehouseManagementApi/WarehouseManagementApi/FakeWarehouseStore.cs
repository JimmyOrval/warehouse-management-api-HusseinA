using WarehouseManagementApi.Models;

namespace WarehouseManagementApi;

public static class FakeWarehouseStore
{
    public static List<Product> Products { get; set; }

    static FakeWarehouseStore()
    {
        Products =
        [
            new Product
            {
                Id = "93a46de1-6427-486c-bdca-5faaa11da0d8",
                Name = "Laptop 1",
                Sku = "LAPTOP-SKU-1",
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
            },

            new Product
            {
            Id = "df7b864c-80c5-4988-ab4d-aa5ac72515ee",
            Name = "Scanner",
            Sku = "SCANNER-SKU",
            Description = "Logitec Scanner",
            Price = 500.00m,
            QuantityInStock = 70,
            SupplierName = "Supplier1",
            ExpiryDate = DateTime.Now.AddMonths(4),
            IsArchived = true,
            CreatedAt = DateTime.Now.AddDays(-4),
            LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "d7853aff-8384-4323-9c24-5dedb633c7f7",
                Name = "Laptop 2",
                Sku = "LAPTOP-SKU-2",
                Description = "Lenovo Laptop",
                Price = 2000.00m,
                QuantityInStock = 5,
                SupplierName = "Supplier4",
                ExpiryDate = DateTime.Now.AddMonths(2),
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-3),
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "e5ab4931-b457-499a-9591-dc3e642e1acd",
                Name = "Printer",
                Sku = "Printer-SKU",
                Description = "HP Printer",
                Price = 350.00m,
                QuantityInStock = 20,
                SupplierName = "Supplier2",
                ExpiryDate = DateTime.Now.AddMonths(10),
                IsArchived = true,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "230217c7-9747-49d6-8c3a-b3e9c027208a",
                Name = "Monitor 1",
                Sku = "MONITOR-SKU-1",
                Description = "MSI Monitor 1",
                Price = 250.00m,
                QuantityInStock = 10,
                SupplierName = "Supplier3",
                ExpiryDate = DateTime.Now.AddMonths(7),
                IsArchived = false,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "f4bc67d3-0426-4cb7-bc93-35348e0a05b7",
                Name = "Monitor 2",
                Sku = "MONITOR-SKU-2",
                Description = "MSI Monitor 2",
                Price = 300.00m,
                QuantityInStock = 5,
                SupplierName = "Supplier4",
                ExpiryDate = DateTime.Now.AddMonths(7),
                IsArchived = true,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "9f6ee5f1-ae6f-475e-85f9-8360da21b23a",
                Name = "Monitor 3",
                Sku = "MONITOR-SKU-3",
                Description = "MSI Monitor 3",
                Price = 500.00m,
                QuantityInStock = 15,
                SupplierName = "Supplier1",
                ExpiryDate = DateTime.Now.AddMonths(7),
                IsArchived = false,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            },
            
            new Product
            {
                Id = "35273d75-6aa4-43fe-a60f-4011489873fb",
                Name = "Laptop 3",
                Sku = "LAPTOP-SKU-3",
                Description = "Apple Macbook Laptop",
                Price = 3000.00m,
                QuantityInStock = 3,
                SupplierName = "Supplier5",
                ExpiryDate = DateTime.Now.AddMonths(20),
                IsArchived = true,
                CreatedAt = DateTime.Now.AddDays(-1),
                LastUpdatedAt = DateTime.Now
            }
        ];
    }
}