namespace Application.DTOs;

public record WarehouseItemDto(
    string Id,
    string ProductId,
    string Location,
    int QuantityInStock,
    DateTime LastStockUpdate
);