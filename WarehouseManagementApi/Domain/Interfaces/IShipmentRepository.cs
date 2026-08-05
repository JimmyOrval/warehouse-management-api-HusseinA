using Domain.Models;

namespace Domain.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(string id, CancellationToken cancellationToken);
    void Add(Shipment shipment);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
