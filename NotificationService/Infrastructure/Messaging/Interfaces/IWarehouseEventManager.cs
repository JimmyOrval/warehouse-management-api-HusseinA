namespace Infrastructure.Messaging.Interfaces;

public interface IWarehouseEventManager
{
    Task HandleMessageAsync(
        string routingKey,
        byte[] body,
        CancellationToken cancellationToken);
}