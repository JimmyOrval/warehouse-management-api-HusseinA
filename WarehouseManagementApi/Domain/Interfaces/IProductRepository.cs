using Domain.Models;

namespace Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<List<Product>> GetAvailableAsync(CancellationToken cancellationToken);
    
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    
    Task<List<Product>> SearchAsync(string? name, string? supplier, CancellationToken cancellationToken);
    
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken);
    
    void Add(Product product);
    
    void Delete(Product product);

    Task SaveChangesAsync(CancellationToken cancellationToken);
    
    Task<List<Product>> GetProductsBySupplierAsync(
        string supplierName, bool isAscending, CancellationToken cancellationToken);
    
    Task<List<IGrouping<int, Product>>> GroupByExpiryYearAsync(
        CancellationToken cancellationToken);

    public record ExpiryYearCountry(int Year, string Country);
    Task<List<IGrouping<ExpiryYearCountry, Product>>>
        GroupByExpiryYearAndSupplierCountryAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);
    
    Task<int> GetArchivedCountAsync(CancellationToken cancellationToken);
    
    Task<List<Product>> GetPagedProductsAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<List<Product>> GetExpiringOrExpiredAsync(DateTime date, CancellationToken cancellationToken);

    Task<List<Product>> GetExpiredSinceAsync(DateTime date, CancellationToken cancellationToken);
    
    void AddImage(ProductImage image);
    
    void DeleteImage(ProductImage image);
    
    Task<ProductImage?> GetImageByIdAsync(string imageId, CancellationToken cancellationToken);
    
    Task<List<WarehouseItem>> GetProductWarehouseItemsAsync(
        string productId, CancellationToken cancellationToken);
     
    Task<int> GetTotalStockQuantityAsync(string productId, CancellationToken cancellationToken);
    
    Task<List<Product>> GetExpiringSoonAsync(
        DateTime from, DateTime to,
        CancellationToken cancellationToken);
    
    Task<List<Product>> GetOutOfStockAsync(CancellationToken cancellationToken);
}