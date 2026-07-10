using Application.ViewModels;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductQuantity;

public record UpdateProductQuantityCommand(string Id, int Quantity, string Location)
    : IRequest<WarehouseItemViewModel>;