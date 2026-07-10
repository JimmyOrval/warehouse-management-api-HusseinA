using Domain.Interfaces;
using Domain.Models;
using WarehouseManagementApi;

namespace Infrastructure.Repositories;

public class SupplierRepository(WarehouseDbContext context) : ISupplierRepository
{
    public IEnumerable<Supplier> GetAll()
    {
        return context.Suppliers.ToList();
    }

    public Supplier? GetById(string id)
    {
        return context.Suppliers.FirstOrDefault(s => s.Id.Equals(id));
    }

    public string Add(Supplier supplier)
    {
        context.Suppliers.Add(supplier);
        return supplier.Id;
    }

    public void Delete(Supplier supplier)
    {
        context.Suppliers.Remove(supplier);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}