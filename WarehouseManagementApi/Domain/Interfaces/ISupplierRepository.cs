using Domain.Models;

namespace Domain.Interfaces;

public interface ISupplierRepository
{
    IQueryable<Supplier> GetAll();
    
    Supplier? GetById(string id);

    void Add(Supplier supplier);
    
    void Delete(Supplier supplier);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}