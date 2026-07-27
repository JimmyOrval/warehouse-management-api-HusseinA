using Application.Features.Suppliers.Commands.CreateSupplier;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Suppliers;

public class CreateSupplierTests
{
    [Fact]
    public async Task Create_ValidSupplier_Succeeds()
    {
        var repository = RepositoryMockHelper.MockRepository<ISupplierRepository>();
        await HandleSupplierCreation(repository);
        repository.Verify(r => r.Add(It.IsAny<Supplier>()), Times.Once);
    }
    
    private static async Task HandleSupplierCreation(Mock<ISupplierRepository> repository)
    {
        var supplier = new SupplierBuilder().Build();
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<CreateSupplierCommand, Supplier>();
        });
        
        var handler = new CreateSupplierCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<CreateSupplierCommandHandler>());
        
        var command = new CreateSupplierCommand(
            supplier.Name,
            supplier.Country,
            supplier.ContactEmail,
            supplier.Phone);

        await handler.Handle(command, CancellationToken.None);
    }
}