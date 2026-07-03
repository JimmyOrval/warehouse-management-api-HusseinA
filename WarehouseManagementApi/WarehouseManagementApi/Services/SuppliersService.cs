using WarehouseManagementApi.Contracts;
using WarehouseManagementApi.Models;

namespace WarehouseManagementApi.Services;

public class SuppliersService : ISuppliersService
{
    public List<Supplier> GetSuppliers()
    {
        return FakeSupplierDirectory.Suppliers.ToList();
    }

    public Supplier? GetSupplier(string id)
    {
        return FakeSupplierDirectory.Suppliers
            .FirstOrDefault(s => s.Id.Equals(id));
    }

    public Supplier CreateSupplier(CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name =  request.Name,
            Country =  request.Country,
            ContactEmail = request.ContactEmail,
            Phone =  request.Phone,
            IsActive = true
        };
        
        FakeSupplierDirectory.Suppliers.Add(supplier);
        return supplier;
    }

    public Supplier? DeleteSupplier(string id)
    {
        var supplier = FakeSupplierDirectory.Suppliers
            .FirstOrDefault(s => s.Id.Equals(id));

        if (supplier == null)
            return null;

        supplier.IsActive = false;
        return supplier;
    }
}