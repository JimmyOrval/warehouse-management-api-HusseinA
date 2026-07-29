namespace Domain.Events;

public record WarehouseEvent
{
    public required string EventId { get; init; }
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required string Severity { get; init; }
    public required string RelatedEntityId { get; init; }
    public required string RelatedEntity { get; init; }
}