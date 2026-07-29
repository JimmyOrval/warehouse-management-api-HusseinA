using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.WarehouseItems.Commands.DeleteWarehouseItem;

public class DeleteWarehouseItemCommandHandler(
    IWarehouseItemRepository warehouseItemRepository,
    ILogger<DeleteWarehouseItemCommandHandler> logger)
    : IRequestHandler<DeleteWarehouseItemCommand>
{
    public async Task Handle(DeleteWarehouseItemCommand request, CancellationToken cancellationToken)
    {
        var item = await warehouseItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item == null)
        {
            logger.LogWarning("Item {ItemId} not found", request.Id);
            throw new NotFoundException($"Item {request.Id} not found");
        }
        
        warehouseItemRepository.Delete(item);
        await warehouseItemRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Item {ItemId} successfully deleted", request.Id);
    }
}