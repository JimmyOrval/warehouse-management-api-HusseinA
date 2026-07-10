using Domain.Interfaces;
using Domain.Models;
using WarehouseManagementApi;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    public IEnumerable<Product> GetAll()
    {
        var products = FakeWarehouseStore.Products.AsEnumerable();
        return products.OrderByDescending(p => p.CreatedAt).ToList();
    }

    public IEnumerable<Product> GetAvailable()
    {
        var items = FakeWarehouseStore.Items
            .ToLookup(i => i.ProductId, i => i.QuantityInStock);

        return FakeWarehouseStore.Products
            .Where(p => !p.IsArchived && items[p.Id].Sum() > 0)
            .OrderByDescending(p => p.CreatedAt);
    }

    public Product? GetById(string id)
    {
        return FakeWarehouseStore.Products.FirstOrDefault(x => x.Id.Equals(id));
    }

    public IEnumerable<Product> Search(string? name, string? supplier)
    {
        var filteredProducts = FakeWarehouseStore.Products.AsEnumerable();

        // if name filter available, filter according to name
        if(!string.IsNullOrWhiteSpace(name))
        {
            filteredProducts = filteredProducts.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        // if name filter available, filter according to supplier
        if(!string.IsNullOrWhiteSpace(supplier))
        {
            filteredProducts = filteredProducts.Where(p => 
                FakeSupplierDirectory.Suppliers.Any(s => 
                    s.Id == p.SupplierId && 
                    s.Name.Contains(supplier, StringComparison.OrdinalIgnoreCase)
                )
            );
        }
        return filteredProducts;
    }

    public bool SkuExists(string sku)
    {
        return FakeWarehouseStore.Products
            .Any(p => p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(Product product)
    {
        FakeWarehouseStore.Products.Add(product);
    }

    public void Update(Product product)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public WarehouseItem? GetWarehouseItem(string productId, string location)
    {
        return FakeWarehouseStore.Items.FirstOrDefault(i =>
            i.ProductId.Equals(productId) &&
            i.Location.Equals(location, StringComparison.OrdinalIgnoreCase));
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
        return FakeWarehouseStore.Items
            .Where(i => i.ProductId == productId)
            .Sum(i => i.QuantityInStock);
    }
}