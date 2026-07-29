namespace Infrastructure.Messaging.RabbitMQ;

public class RabbitMqSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Exchange { get; init; } = string.Empty;
    public string Queue { get; init; } = string.Empty;
    public List<string> RoutingKeys { get; init; } = [];
    public string DeadLetterExchange { get; init; } = "warehouse.events.dlx";
    public string DeadLetterQueue { get; init; } = "notifications.warehouse-events.dlq";
}