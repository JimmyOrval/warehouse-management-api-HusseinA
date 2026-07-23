using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(WarehouseDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetAvailableAsync(CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .Where(p =>
                p.Status == ProductStatus.Active &&
                context.WarehouseItems
                    .Where(w => w.ProductId == p.Id)
                    .Sum(w => (int?)w.QuantityInStock) > 0
            )
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Product>> SearchAsync(
        string? name, string? supplier,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> products = context.Products.Include(p => p.Supplier);

        // if name filter available, filter according to name
        if(!string.IsNullOrWhiteSpace(name))
        {
            products = products
                .Where(p => 
                    EF.Functions.ILike(
                        p.Name,
                        $"{name}%"));
        }

        // if name filter available, filter according to supplier
        if(!string.IsNullOrWhiteSpace(supplier))
        {
            products = products
                .Where(p => 
                context.Suppliers.Any(s => 
                    s.Id == p.SupplierId &&
                    EF.Functions.ILike(s.Name, $"{supplier}%")
                )
            );
        }
        return await products.ToListAsync(cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(string sku,
        CancellationToken cancellationToken)
    {
        return await context.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<Product>> GetProductsBySupplierAsync(
        string supplierName,
        bool isAscending,
        CancellationToken cancellationToken)
    {
        var products = context.Products
            .Include(p => p.Supplier)
            .Where(p => p.Supplier != null &&
                        EF.Functions.ILike(p.Supplier.Name, $"{supplierName}%"));

        products = isAscending ? products.OrderBy(p => p.CreatedAt) : products.OrderByDescending(p => p.CreatedAt);

        return await products.ToListAsync(cancellationToken);
    }
    
    public async Task<List<IGrouping<int, Product>>> GroupByExpiryYearAsync(
        CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .GroupBy(p => p.ExpiryDate.Year)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<IGrouping<IProductRepository.ExpiryYearCountry, Product>>>
        GroupByExpiryYearAndSupplierCountryAsync(CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .GroupBy(p => new IProductRepository.ExpiryYearCountry
                (p.ExpiryDate.Year, p.Supplier!.Country ))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return await context.Products.CountAsync(cancellationToken);
    }
    
    public async Task<int> GetArchivedCountAsync(
        CancellationToken cancellationToken)
    {
        return await context.Products
            .CountAsync(
                p => p.Status == ProductStatus.Archived,
                cancellationToken);
    }

    public async Task<List<Product>> GetPagedProductsAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Supplier)
            .OrderBy(p => p.Id)
            // skips products according to page number and its size
            .Skip((pageNumber - 1) * pageSize)
            // then takes products as much as page can hold
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetExpiringOrExpiredAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await context.Products
            .Where(p => p.Status != ProductStatus.Archived
                        && p.ExpiryDate <= date)
            .ToListAsync(cancellationToken);
    }

    // same as above, but definition and usage differ
    public async Task<List<Product>> GetExpiredSinceAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await context.Products
            .Where(p => p.Status != ProductStatus.Archived 
                        && p.ExpiryDate > date)
            .ToListAsync(cancellationToken);
    }

    public void AddImage(ProductImage image)
    {
        context.ProductImages.Add(image);
    }

    public void DeleteImage(ProductImage image)
    {
        context.ProductImages.Remove(image);
    }

    public async Task<ProductImage?> GetImageByIdAsync(string imageId, CancellationToken cancellationToken)
    {
        return await context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken);
    }

    public async Task<List<WarehouseItem>> GetProductWarehouseItemsAsync(string productId, CancellationToken cancellationToken)
    {
        return await context.WarehouseItems
            .Where(i => i.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
    
    public int GetTotalStockQuantity(string productId)
    {
        return context.WarehouseItems
            .Where(i => i.ProductId == productId)
            .Sum(i => i.QuantityInStock);
    }
}