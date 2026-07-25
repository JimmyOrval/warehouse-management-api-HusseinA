using System.Text;
using System.Text.Json;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Infrastructure.Messaging;

public class RabbitMqEventPublisher()
    : IEventPublisher, IAsyncDisposable
{
    public required RabbitMqSettings Settings;
    public required ILogger<RabbitMqEventPublisher> Logger;
    private IConnection? _connection;
    // this is to handle multiple connection attempts
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMqEventPublisher(
        IOptions<RabbitMqSettings> options,
        ILogger<RabbitMqEventPublisher> logger) : this()
    {
        Logger = logger;
        Settings = options.Value;
    }
    
    /*
     * When we want to publish a message, we check connection to RabbitMQ
     * If we're not already connected, we create the connection
     * We make sure 1 message can try to connect at a time
     * If we still can't connect, the actual action (product creation)
     * continues normally, but we don't send a message for it
     * If we connect, we open a channel and send the message through it
     * When we're done, we close the channel and keep the connection for new messages
     */
    
    
    private async Task<IConnection?> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
            return _connection;
        
        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = Settings.Host,
                Port = Settings.Port,
                UserName = Settings.Username,
                Password = Settings.Password
            };
            
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        catch (Exception e)
        {
            Logger.LogWarning(e, "Cannot connect to RabbitMQ. Publishing failed.");
            throw new ConnectFailureException("Cannot connect to RabbitMQ. Publishing failed.", e);
        }
        finally
        {
            _connectionLock.Release();
        }
    }
    
    public async Task PublishAsync<TEvent>(
        TEvent warehouseEvent,
        string routingKey,
        CancellationToken cancellationToken)
        where TEvent : WarehouseEvent
    {
        var connection = await ConnectAsync(cancellationToken);

        if (connection is null)
        {
            Logger.LogWarning("Cannot connect to RabbitMQ. Publishing failed.");
            return;
        }

        try
        {
            await using var channel = await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);
            
            await channel.ExchangeDeclareAsync(
                exchange: Settings.Exchange, 
                type: ExchangeType.Topic, 
                durable: true,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(warehouseEvent, warehouseEvent.GetType());
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: Settings.Exchange, 
                routingKey: routingKey, 
                mandatory: false,
                basicProperties: properties, 
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Cannot publish event {EventId} {RoutingKey} to RabbitMQ.",
                warehouseEvent.EventId, routingKey);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            try
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error closing RabbitMQ connection.");
            }

            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}