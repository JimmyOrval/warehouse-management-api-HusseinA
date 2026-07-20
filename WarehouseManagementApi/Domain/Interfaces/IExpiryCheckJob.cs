namespace Domain.Interfaces;

public interface IExpiryCheckJob
{
    Task CheckExpiringProductsAsync(CancellationToken cancellationToken);
}