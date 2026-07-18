using Application.Jobs;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application.Jobs;

public class ExpiryCheckJobTests
{
    [Fact]
    public async Task CheckExpiringProductsAsync_ArchivesProductsExpiredOverSevenDays()
    {
        var overdueProduct = CreateTestProduct(
            expiryDate: DateTime.UtcNow.AddDays(-10));

        var mockRepository = new Mock<IProductRepository>();
        
        mockRepository
            .Setup(r => r.GetExpiringOrExpiredAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([overdueProduct]);
        
        mockRepository
            .Setup(r => r.GetExpiredOlderThanDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([overdueProduct]);

        var cacheMock = new Mock<IDistributedCache>();
        var loggerMock = new Mock<ILogger<ExpiryCheckJob>>();

        var job = new ExpiryCheckJob(mockRepository.Object, cacheMock.Object, loggerMock.Object);

        await job.CheckExpiringProductsAsync(CancellationToken.None);

        Assert.Equal(ProductStatus.Archived, overdueProduct.Status);
        mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckExpiringProductsAsync_WhenNoOverdueProducts_DoesNotCallSaveChanges()
    {
        var repositoryMock = new Mock<IProductRepository>();
        
        repositoryMock
            .Setup(r => r.GetExpiringOrExpiredAsync(It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        
        repositoryMock
            .Setup(r => r.GetExpiredOlderThanDateAsync(It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var job = new ExpiryCheckJob(
            repositoryMock.Object, Mock.Of<IDistributedCache>(),
            Mock.Of<ILogger<ExpiryCheckJob>>());

        await job.CheckExpiringProductsAsync(CancellationToken.None);

        repositoryMock.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Product CreateTestProduct(DateTime expiryDate) =>
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Expiring Product",
            Sku = "SKU-002",
            Description = "Test",
            Price = 20m,
            SupplierId = Guid.NewGuid().ToString(),
            ExpiryDate = expiryDate,
        };
}