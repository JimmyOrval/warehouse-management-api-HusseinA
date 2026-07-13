using Domain.Models;

namespace Domain.Interfaces;

public interface ISupplierRepository
{
    IEnumerable<Supplier> GetAll();
    
    Supplier? GetById(string id);

    string Add(Supplier supplier);
    
    void Delete(string id);
}