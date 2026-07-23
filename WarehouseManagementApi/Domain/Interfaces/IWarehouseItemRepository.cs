using Domain.Models;

namespace Domain.Interfaces;

public interface IWarehouseItemRepository
{
    Task<List<WarehouseItem>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<WarehouseItem?> GetByIdAsync(string itemId,
        CancellationToken cancellationToken);
    
    void Add(WarehouseItem warehouseItem);
    
    void Delete(WarehouseItem warehouseItem);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
    
    Task<List<StockMovement>> GetStockMovementsAsync(
        string itemId,
        CancellationToken cancellationToken);
    
    Task<bool> LocationExistsAsync(string location, CancellationToken cancellationToken);
}