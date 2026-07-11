using Domain.Models;

namespace Domain.Interfaces;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    
    IQueryable<Product> GetAvailable();
    
    Product? GetById(string id);
    
    IQueryable<Product> Search(string? name, string? supplier);
    
    bool SkuExists(string sku);
    
    void Add(Product product);
    
    void Delete(Product product);

    void SaveChanges();

    WarehouseItem? GetWarehouseItem(string productId, string location);
    
    IEnumerable<WarehouseItem> GetWarehouseItems(string productId);
    
    void UpdateWarehouseItem(WarehouseItem warehouseItem);
    
    int GetQuantity(string productId);
    
    IQueryable<Product> GetProductsBySupplier(string supplierName, bool isAscending);
    
    IQueryable<IGrouping<int, Product>> GroupByExpiryYear();

    public IQueryable<IGrouping<object, Product>> GroupByExpiryYearAndSupplierCountry();

    int GetCount();
    
    IQueryable<Product> GetPagedProducts(int pageNumber, int pageSize);
}