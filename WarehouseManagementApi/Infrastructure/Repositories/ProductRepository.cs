using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(WarehouseDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAll()
    {
        return await context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public IQueryable<Product> GetAvailable()
    {
        return context.Products
            .Where(p =>
                !p.IsArchived &&
                context.WarehouseItems
                    .Where(w => w.ProductId == p.Id)
                    .Sum(w => (int?)w.QuantityInStock) > 0
            )
            .OrderByDescending(p => p.CreatedAt);
    }

    public async Task<Product?> GetById(string id)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    public IQueryable<Product> Search(string? name, string? supplier)
    {
        IQueryable<Product> products = context.Products;

        // if name filter available, filter according to name
        if(!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p =>
                // ILike ignores case, since EFCore can't
                // translate OrdinalIgnoreCase into SQL
                EF.Functions.ILike(p.Name, $"{name}%"));
        }

        // if name filter available, filter according to supplier
        if(!string.IsNullOrWhiteSpace(supplier))
        {
            products = products.Where(p => 
                context.Suppliers.Any(s => 
                    s.Id == p.SupplierId &&
                    EF.Functions.ILike(s.Name, $"{supplier}%")
                )
            );
        }
        return products;
    }

    public async Task<bool> SkuExists(string sku)
    {
        return await context.Products.AnyAsync(p => p.Sku == sku);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
    
    public IQueryable<Product> GetProductsBySupplier(string supplierName, bool isAscending)
    {
        var products = context.Products
            .Where(p => p.Supplier != null &&
                        p.Supplier.Name == supplierName);

        products = isAscending ? products.OrderBy(p => p.CreatedAt) : products.OrderByDescending(p => p.CreatedAt);

        return products;
    }

    // I chose IQueryable<> instead of IEnumerable<> since the filtering is
    // happening with SQL rather than in-memory like in the above method
    public IQueryable<IGrouping<int, Product>> GroupByExpiryYear()
    {
        return context.Products.GroupBy(p => p.ExpiryDate.Year);
    }

    public IQueryable<IGrouping<object, Product>> GroupByExpiryYearAndSupplierCountry()
    {
        return context.Products
            .GroupBy(p => new
            {
                p.ExpiryDate.Year,
                p.Supplier!.Country
            });
    }

    public async Task<int> GetCount()
    {
        return await context.Products.CountAsync();
    }

    public IQueryable<Product> GetPagedProducts(int pageNumber, int pageSize)
    {
        return context.Products
            .OrderBy(p => p.Id)
            // skips products according to page number and its size
            .Skip((pageNumber - 1) * pageSize)
            // then takes products as much as page can hold
            .Take(pageSize);
    }

    // I will later create a separate WarehouseItem
    // repository for the following methods
    public WarehouseItem? GetWarehouseItem(string productId, string location)
    {
        return context.WarehouseItems.FirstOrDefault(i =>
            i.ProductId == productId &&
            EF.Functions.ILike(i.Location, $"{location}%"));
    }

    public IEnumerable<WarehouseItem> GetWarehouseItems(string productId)
    {
        throw new NotImplementedException();
    }

    public void UpdateWarehouseItem(WarehouseItem warehouseItem)
    {
        throw new NotImplementedException();
    }
    
    public int GetQuantity(string productId)
    {
        return context.WarehouseItems
            .Where(i => i.ProductId == productId)
            .Sum(i => i.QuantityInStock);
    }
}