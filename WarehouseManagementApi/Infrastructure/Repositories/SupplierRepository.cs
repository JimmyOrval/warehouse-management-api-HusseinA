using Domain.Interfaces;
using Domain.Models;
using WarehouseManagementApi;

namespace Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    public IEnumerable<Supplier> GetAll()
    {
        return FakeSupplierDirectory.Suppliers.ToList();
    }

    public Supplier? GetById(string id)
    {
        return FakeSupplierDirectory.Suppliers.FirstOrDefault(s => s.Id.Equals(id));
    }

    public string Add(Supplier supplier)
    {
        FakeSupplierDirectory.Suppliers.Add(supplier);
        return supplier.Id;
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }
}