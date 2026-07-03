using WarehouseManagementApi.Contracts;
using WarehouseManagementApi.Models;

namespace WarehouseManagementApi.Services;

public interface ISuppliersService
{
    public List<Supplier> GetSuppliers();
    public Supplier? GetSupplier(string id);
    public Supplier CreateSupplier(CreateSupplierRequest request);
    public Supplier? DeleteSupplier(string id);
}