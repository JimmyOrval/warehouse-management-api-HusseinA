using Application.Features.Products.Commands.CreateProduct;
using Application.Mappings;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Moq;

namespace Tests.Application.Commands;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        var repository = new Mock<IProductRepository>();
        
        var mockMapper = new Mock<IMapper>(); 

        mockMapper
            .Setup(m => m.Map<Product>(It.IsAny<CreateProductCommand>()))
            .Returns((CreateProductCommand src) => new Product 
            { 
                Id = Guid.NewGuid().ToString(),
                Sku = src.Sku,
                Name = src.Name,
                Description = src.Description,
                Price = src.Price,
                ExpiryDate =  src.ExpiryDate,
                SupplierId =  src.SupplierId,
            });

        repository
            .Setup(r => r.SkuExistsAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(false);

        repository
            .Setup(r => r.Add(It.IsAny<Product>()));

        var handler = new CreateProductCommandHandler(repository.Object, mockMapper.Object);

        var command = new CreateProductCommand
        {
            Name = "Laptop",
            Sku = "SKU1",
            Description = "HP",
            Price = 1000,
            SupplierId = Guid.NewGuid().ToString(),
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await handler.Handle(command, CancellationToken.None);

        repository.Verify(r =>
                r.Add(It.IsAny<Product>()),
            Times.Once);
    }
}