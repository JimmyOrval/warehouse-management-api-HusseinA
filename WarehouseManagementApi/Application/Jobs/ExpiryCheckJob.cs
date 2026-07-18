using Application.Common;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Jobs;

public class ExpiryCheckJob(
    IProductRepository productRepository,
    IDistributedCache cache,
    ILogger<ExpiryCheckJob> logger)
    : IExpiryCheckJob
{
    public async Task CheckExpiringProductsAsync(CancellationToken cancellationToken)
    {
        var date = DateTime.UtcNow.AddDays(30);
        var products = await productRepository.GetExpiringOrExpiredAsync(date, cancellationToken);
        
        var expired = products.Where(p => p.ExpiryDate < DateTime.UtcNow).ToList();
        var expiringSoon = products.Except(expired).ToList();
        
        logger.LogInformation(
            "Expiry check complete. {ExpiryCount} expired, {ExpiringSoonCount} expiring within 30 days",
            expired.Count, expiringSoon.Count);

        if (expired.Count > 0)
        {
            logger.LogInformation("Expired products: {ProductNames}",
                string.Join(", ", expired.Select(p => p.Name)));
        }

        if (expiringSoon.Count > 0)
        {
            logger.LogInformation("Products expiring within 30 days: {ProductNames}",
                string.Join(", ", expiringSoon.Select(p => p.Name)));
        }
        
        await ArchiveExpiredProductsAsync(cancellationToken);
    }

    private async Task ArchiveExpiredProductsAsync(CancellationToken cancellationToken)
    {
        var dateLimit = DateTime.UtcNow.AddDays(-7);
        var archivableProducts = await productRepository.GetExpiredOlderThanDateAsync(dateLimit, cancellationToken);
        
        foreach(var product in archivableProducts)
        {
            product.Archive();
            logger.LogInformation("Product {ProductId} ({ProductName}) auto-archived." +
                                  "Expired since {ExpiryDate}",
                product.Id, product.Name, product.ExpiryDate);
            await cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        }

        if (archivableProducts.Count > 0)
        {
            await productRepository.SaveChangesAsync(cancellationToken);
            foreach(var key in ProductCacheKeys.ListVariations)
                await cache.RemoveAsync(key, cancellationToken);
            
            logger.LogInformation(
                "Auto-archived {ArchivedCount} product(s) expired more than 7 days ago.",
                archivableProducts.Count);
        }
    }
}