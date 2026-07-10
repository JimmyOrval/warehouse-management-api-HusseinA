using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(WarehouseDbContext context) : IProductRepository
{
    public IEnumerable<Product> GetAll()
    {
        var products = context.Products.AsEnumerable();
        return products.OrderByDescending(p => p.CreatedAt).ToList();
    }

    public IEnumerable<Product> GetAvailable()
    {
        var items = context.WarehouseItems
            .ToLookup(i => i.ProductId, i => i.QuantityInStock);

        return context.Products
            .Where(p => !p.IsArchived && items[p.Id].Sum() > 0)
            .OrderByDescending(p => p.CreatedAt);
    }

    public Product? GetById(string id)
    {
        return context.Products.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Product> Search(string? name, string? supplier)
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

    public bool SkuExists(string sku)
    {
        return context.Products.Any(p => p.Sku == sku);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
    
    public IEnumerable<Product> GetProductsBySupplier(string supplierName, bool isAscending)
    {
        var products = context.Products
            .Where(p => p.Supplier!.Name == supplierName);

        products = isAscending ? products.OrderBy(p => p.CreatedAt) : products.OrderByDescending(p => p.CreatedAt);

        return products;
    }

    // I chose IQueryable<> instead of IEnumerable<> since the filtering is
    // happening with SQL rather than in-memory like in the above method
    public IQueryable<IGrouping<int, Product>> GroupByExpiryYear()
    {
        return context.Products.GroupBy(p => p.ExpiryDate.Year);
    }

    public IEnumerable<Product> GroupByExpiryYearAndSupplierCountry()
    {
        var products = context.Products
            // used an anonymous object to simplify table joining
            .GroupBy(p => new {p.ExpiryDate.Year, p.Supplier!.Country});
        
        return products.SelectMany(group => group).ToList();
    }

    public int GetCount()
    {
        return context.Products.Count();
    }

    public IEnumerable<Product> GetPagedProducts(int pageNumber, int pageSize)
    {
        return context.Products
            // skips products according to page number and its size
            .Skip((pageNumber - 1) * pageSize)
            // then takes products as much as page can hold
            .Take(pageSize).AsEnumerable();
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