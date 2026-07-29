namespace Domain.Events;

public record WarehouseEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString();
    public required string CorrelationId { get; init; }
    public DateTime EventTime { get; init; } = DateTime.UtcNow;
    public required string EventType { get; init; }
    public required string Severity { get; init; }
    public required string RelatedEntityId { get; init; }
    public required string RelatedEntityType { get; init; }
}