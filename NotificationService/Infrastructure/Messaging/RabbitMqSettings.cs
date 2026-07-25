namespace Infrastructure.Messaging;

public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public string Queue { get; set; } = string.Empty;
    public List<string> RoutingKeys { get; set; } = [];
    public string DeadLetterExchange { get; set; } = "warehouse.events.dlx";
    public string DeadLetterQueue { get; set; } = "notifications.warehouse-events.dlq";
}