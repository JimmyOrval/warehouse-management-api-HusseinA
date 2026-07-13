using Domain.Models;

namespace Domain.Interfaces;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<Supplier?> GetByIdAsync(string id, CancellationToken cancellationToken);

    void Add(Supplier supplier);
    
    void Delete(Supplier supplier);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}