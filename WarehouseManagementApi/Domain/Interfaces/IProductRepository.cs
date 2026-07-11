using Domain.Models;

namespace Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAll();
    
    IQueryable<Product> GetAvailable();
    
    Task<Product?> GetById(string id);
    
    IQueryable<Product> Search(string? name, string? supplier);
    
    Task<bool> SkuExists(string sku);
    
    void Add(Product product);
    
    void Delete(Product product);

    Task SaveChangesAsync();
    
    IQueryable<Product> GetProductsBySupplier(string supplierName, bool isAscending);
    
    IQueryable<IGrouping<int, Product>> GroupByExpiryYear();

    IQueryable<IGrouping<object, Product>> GroupByExpiryYearAndSupplierCountry();

    Task<int> GetCount();
    
    IQueryable<Product> GetPagedProducts(int pageNumber, int pageSize);
    
    WarehouseItem? GetWarehouseItem(string productId, string location);
                                                                                 
    IEnumerable<WarehouseItem> GetWarehouseItems(string productId);
                                                                                 
    void UpdateWarehouseItem(WarehouseItem warehouseItem);
     
    int GetQuantity(string productId);
}