using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementApi;

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