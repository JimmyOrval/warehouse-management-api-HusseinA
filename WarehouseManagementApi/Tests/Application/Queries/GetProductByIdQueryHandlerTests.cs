using Application.Features.Products.Queries.GetProductById;
using Domain.Interfaces;
using Domain.Models;
using Moq;

namespace Tests.Application.Queries;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenProductDoesNotExist()
    {
        var repository = new Mock<IProductRepository>();

        repository
            .Setup(r => r.GetById(It.IsAny<string>()))
            .Returns((Product?)null);

        var handler = new GetProductByIdQueryHandler(repository.Object);

        var query = new GetProductByIdQuery(Guid.NewGuid().ToString());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(query, CancellationToken.None));
    }
}