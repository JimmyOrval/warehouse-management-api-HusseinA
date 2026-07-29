using RabbitMQ.Client;

namespace Infrastructure.Messaging.Interfaces;

public interface IRabbitMqConnectionManager
{
    Task<IChannel> ConnectAsync(CancellationToken cancellationToken);
}