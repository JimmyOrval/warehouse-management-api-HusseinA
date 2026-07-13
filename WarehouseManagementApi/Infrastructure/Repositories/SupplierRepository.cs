using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SupplierRepository(WarehouseDbContext context) : ISupplierRepository
{
    public async Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Suppliers.ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public void Add(Supplier supplier)
    {
        context.Suppliers.Add(supplier);
    }

    public void Delete(Supplier supplier)
    {
        context.Suppliers.Remove(supplier);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}