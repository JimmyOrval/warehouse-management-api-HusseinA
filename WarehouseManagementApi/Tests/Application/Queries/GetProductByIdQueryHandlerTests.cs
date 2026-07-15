using Application.Features.Products.Queries.GetProductById;
using AutoMapper;
using Domain.Exceptions;
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
        var mockMapper = new Mock<IMapper>(); 

        repository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(repository.Object, mockMapper.Object);

        var query = new GetProductByIdQuery(Guid.NewGuid().ToString());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, CancellationToken.None));
    }
}