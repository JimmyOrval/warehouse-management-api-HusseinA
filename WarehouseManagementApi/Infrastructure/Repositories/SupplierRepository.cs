using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories;

public class SupplierRepository(WarehouseDbContext context) : ISupplierRepository
{
    public IQueryable<Supplier> GetAll()
    {
        return context.Suppliers;
    }

    public Supplier? GetById(string id)
    {
        return context.Suppliers.FirstOrDefault(s => s.Id == id);
    }

    public void Add(Supplier supplier)
    {
        context.Suppliers.Add(supplier);
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