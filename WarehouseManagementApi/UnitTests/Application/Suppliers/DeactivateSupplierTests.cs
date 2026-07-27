using Application.Features.Suppliers.Commands.DeactivateSupplier;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Suppliers;

public class DeactivateSupplierTests
{
    [Fact]
    public async Task DeactivateSupplier_KeepsSupplier()
    {
        var supplier = new SupplierBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<ISupplierRepository>();
        repository.Setup(r => r.GetByIdAsync(supplier.Id, CancellationToken.None))
            .ReturnsAsync(supplier);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Supplier, SupplierViewModel>();
        });
        
        var handler = new DeactivateSupplierCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<DeactivateSupplierCommandHandler>());
        
        await handler.Handle(new DeactivateSupplierCommand(supplier.Id), CancellationToken.None);
        
        Assert.False(supplier.IsActive);
    }
}