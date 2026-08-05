using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShipmentRepository(WarehouseDbContext context) : IShipmentRepository
{
    public async Task<Shipment?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await context.Shipments
            .Include(s => s.Supplier)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public void Add(Shipment shipment)
    {
        context.Shipments.Add(shipment);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
