using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WarehouseItemRepository(WarehouseDbContext context) : IWarehouseItemRepository
{
    public async Task<List<WarehouseItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.WarehouseItems.ToListAsync(cancellationToken);
    }

    public async Task<WarehouseItem?> GetByIdAsync(string itemId, CancellationToken cancellationToken)
    {
        return await context.WarehouseItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == itemId,
                cancellationToken);
    }

    public void Add(WarehouseItem warehouseItem)
    {
        context.WarehouseItems.Add(warehouseItem);
    }

    public void Delete(WarehouseItem warehouseItem)
    {
        context.WarehouseItems.Remove(warehouseItem);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<StockMovement>> GetStockMovementsAsync(string itemId, CancellationToken cancellationToken)
    {
        return await context.StockMovements
            .Where(m => m.WarehouseItemId == itemId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(string location, CancellationToken cancellationToken)
    {
        return await context.WarehouseItems.AnyAsync(
            i => i.Location == location,
            cancellationToken);
    }
}