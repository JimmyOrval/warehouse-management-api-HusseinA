using Domain.Models;

namespace Infrastructure.Messaging.Contracts;

public record WarehouseFileUploaded : WarehouseEvent
{
    public required string FileName { get; init; }
    // product image or supplier document
    public required string FileType { get; init; }
    public required string BucketName { get; init; }
}