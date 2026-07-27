using Application.Features.Products.Commands.ArchiveProduct;
using Application.Features.Products.Queries.ListProducts;
using Application.ViewModels;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.Products;

public class ArchiveProductTests
{
    private static (ArchiveProductCommandHandler handler, Product product)
        CreateHandler()
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        
        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });

        return (new ArchiveProductCommandHandler(
            repository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<ArchiveProductCommandHandler>()),
            product);
    }
    
    [Fact]
    public async Task DeletingMarks_Product_AsArchived()
    {
        var (handler, product) = CreateHandler();
        
        await handler.Handle(
            new ArchiveProductCommand(product.Id),
            CancellationToken.None);
        
        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    [Fact]
    public async Task Archived_Item_RemainsInList()
    {
        var product = new ProductBuilder().Build();
        product.Archive();
        
        var mockRepository = RepositoryMockHelper.MockRepository<IProductRepository>();
        mockRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync([product]);

        var mapper = CommonMocksHelper.MockMapper(cfg => 
        {
            cfg.CreateMap<Product, ProductViewModel>();
        });

        var listHandler = new ListProductsQueryHandler(
            mockRepository.Object,
            mapper,
            CommonMocksHelper.MockDistributedCache(),
            CommonMocksHelper.MockCacheStatsTracker(),
            CommonMocksHelper.MockLogger<ListProductsQueryHandler>());
        
        var returnedList = await listHandler.Handle(
            new ListProductsQuery(false), CancellationToken.None);

        Assert.Contains(returnedList, p => p.Status == ProductStatus.Archived);
    }
}