using Domain.Models;

namespace Domain.Interfaces;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    
    IEnumerable<Product> GetAvailable();
    
    Product? GetById(string id);
    
    IEnumerable<Product> Search(string? name, string? supplier);
    
    bool SkuExists(string sku);
    
    void Add(Product product);
    
    void Delete(Product product);

    void SaveChanges();

    WarehouseItem? GetWarehouseItem(string productId, string location);
    
    IEnumerable<WarehouseItem> GetWarehouseItems(string productId);
    
    void UpdateWarehouseItem(WarehouseItem warehouseItem);
    
    int GetQuantity(string productId);
}