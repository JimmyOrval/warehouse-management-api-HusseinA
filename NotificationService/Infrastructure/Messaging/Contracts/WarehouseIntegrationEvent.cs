namespace Infrastructure.Messaging.Contracts;

public record WarehouseIntegrationEvent
{
    public required string EventId { get; init; }
    public required DateTime EventTime { get; init; }
    public required string CorrelationId { get; init; }
    public required string EventType { get; init; }
    public required string RelatedEntityId { get; init; }
    public required string RelatedEntityType { get; init; }
    public required string Severity { get; init; }
}