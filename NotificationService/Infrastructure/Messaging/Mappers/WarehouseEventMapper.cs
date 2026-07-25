using Domain.Events;
using Infrastructure.Messaging.Contracts;

namespace Infrastructure.Messaging.Mappers;

public static class WarehouseEventMapper
{
    public static WarehouseEvent Map(StockLowDetected e) => new()
    {
        EventId = e.EventId,
        Type = "StockLowDetected",
        Title = $"Stock low: {e.ProductName}",
        Message = $"{e.ProductName} quantity dropped to {e.CurrentQuantity}, " +
                          $" below the minimum of {e.MinimumQuantity}",
        Severity = e.Severity,
        RelatedEntityId = e.RelatedEntityId,
        RelatedEntity = e.RelatedEntityType
    };
    
    public static WarehouseEvent Map(WarehouseFileUploaded e) => new()
    {
        EventId = e.EventId,
        Type = "FileUploaded",
        Title = $"File uploaded: {e.FileName}",
        Message = $"A {e.FileType} file {e.FileName} " +
                          $" was uploaded to {e.BucketName}",
        Severity = e.Severity,
        RelatedEntityId = e.RelatedEntityId,
        RelatedEntity = e.RelatedEntityType
    };
    
    public static WarehouseEvent Map(StockAdjusted e) => new()
    {
        EventId = e.EventId,
        Type = "StockAdjusted",
        Title = $"Stock adjusted: {e.ProductName}",
        Message = $"{e.ProductName} quantity changed from " +
                  $" {e.PreviousQuantity} to {e.NewQuantity}",
        Severity = e.Severity,
        RelatedEntityId = e.RelatedEntityId,
        RelatedEntity = e.RelatedEntityType
    };
    
    public static WarehouseEvent Map(ProductCreated e) => new()
    {
        EventId = e.EventId,
        Type = "ProductCreated",
        Title = $"Product created: {e.ProductName}",
        Message = $"Product {e.ProductName} with SKU {e.Sku} was created " +
                  $" and matched with supplier {e.SupplierName}",
        Severity = e.Severity,
        RelatedEntityId = e.RelatedEntityId,
        RelatedEntity = e.RelatedEntityType
    };
}