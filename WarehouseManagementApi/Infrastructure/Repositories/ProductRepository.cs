using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Database.Models;
using WarehouseManagementApi;
using Product = Domain.Models.Product;

namespace Infrastructure.Repositories;

public class ProductRepository(WarehouseDbFirstContext context) : IProductRepository
{
    public IEnumerable<Product> GetProductsBySupplier(string supplierName, bool isAscending)
    {
        var products = context.Products
            .Where(p => p.Supplier.Name == supplierName)
            // domain layer can't know about entities scaffolded into
            // infrastructure layer, so I mapped the DB-generated
            // product entity into the domain one
            .Select(p => new Product
                {
                    Id = p.Id, Name = p.Name, Sku = p.Sku,
                    Description = p.Description, Price = p.Price, 
                    SupplierId = p.SupplierId, ExpiryDate = p.ExpiryDate,
                    IsArchived = p.IsArchived, CreatedAt = p.CreatedAt,
                    LastUpdatedAt = p.LastUpdatedAt
                });

        products = isAscending ? products.OrderBy(p => p.CreatedAt) : products.OrderByDescending(p => p.CreatedAt);

        return products.ToList();
    }

    // I chose IQueryable<> instead of IEnumerable<> since the filtering is
    // happening with SQL rather than in-memory like in the above method
    public IQueryable<IGrouping<int, Product>> GroupByExpiryYear()
    {
        return context.Products.GroupBy(p => p.ExpiryDate.Year,
            p => new Product
            {
                Id = p.Id, Name = p.Name, Sku = p.Sku,
                Description = p.Description, Price = p.Price, 
                SupplierId = p.SupplierId, ExpiryDate = p.ExpiryDate,
                IsArchived = p.IsArchived, CreatedAt = p.CreatedAt,
                LastUpdatedAt = p.LastUpdatedAt
            });
    }

    public IQueryable GroupByExpiryYearAndSupplierCountry()
    {
        return context.Products
            // used an anonymous object to simplify table joining
            .GroupBy(p => new {p.ExpiryDate.Year, p.Supplier.Country},
                p => new Product()
                {
                    Id = p.Id, Name = p.Name, Sku = p.Sku,
                    Description = p.Description, Price = p.Price, 
                    SupplierId = p.SupplierId, ExpiryDate = p.ExpiryDate,
                    IsArchived = p.IsArchived, CreatedAt = p.CreatedAt,
                    LastUpdatedAt = p.LastUpdatedAt
                });
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
            .Take(pageSize)
            .Select(p => new Product
            {
                Id = p.Id, Name = p.Name, Sku = p.Sku,
                Description = p.Description, Price = p.Price, 
                SupplierId = p.SupplierId, ExpiryDate = p.ExpiryDate,
                IsArchived = p.IsArchived, CreatedAt = p.CreatedAt,
                LastUpdatedAt = p.LastUpdatedAt
            });
    }
    
    public IEnumerable<Product> GetAll()
    {
        var products = FakeWarehouseStore.Products.AsEnumerable();
        return products.OrderByDescending(p => p.CreatedAt).ToList();
    }

    public IEnumerable<Product> GetAvailable()
    {
        var products = FakeWarehouseStore.Products.AsEnumerable();
        products = products.Where(p =>
            !p.IsArchived &&
            FakeWarehouseStore.Items
                .Where(i => i.ProductId == p.Id)
                .Sum(i => i.QuantityInStock) > 0);
        return products.OrderByDescending(p => p.CreatedAt).ToList();
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

    public int GetTotalQuantity(string productId)
    {
        throw new NotImplementedException();
    }
}