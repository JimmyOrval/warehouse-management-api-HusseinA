using Application.Features.Products.Commands.CreateProduct;
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

        repository
            .Setup(r => r.SkuExists(It.IsAny<string>()))
            .Returns(false);

        repository
            .Setup(r => r.Add(It.IsAny<Product>()));

        var handler = new CreateProductCommandHandler(repository.Object);

        var command = new CreateProductCommand(
            "Laptop",
            "SKU1",
            "HP",
            1000,
            Guid.NewGuid().ToString(),
            DateTime.Now.AddMonths(6));

        await handler.Handle(command, CancellationToken.None);

        repository.Verify(r =>
                r.Add(It.IsAny<Product>()),
            Times.Once);
    }
}