using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Presentation.Consumers;

public class RabbitMqConsumer(
    IRabbitMqConnectionManager connectionManager,
    IWarehouseEventManager manager,
    IOptions<RabbitMqSettings> options,
    ILogger<RabbitMqConsumer> logger) : BackgroundService
{
    private readonly RabbitMqSettings _settings = options.Value;
    private IChannel? _channel;

    // now this acts as an entry point, without worrying about connection, deserialization, or mapping
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // keep retrying if broker is not running
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _channel = await connectionManager.ConnectAsync(cancellationToken);
                await StartConsuming(cancellationToken);
                break;
            }
            catch (BrokerUnreachableException e)
            {
                logger.LogWarning("RabbitMQ unreachable at {Host}:{Port}," +
                                  " retrying in 5 seconds. Reason: {Reason}",
                    _settings.Host, _settings.Port, e.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
        
        // keeps the consumer listening for incoming events
        await Task.Delay(Timeout.Infinite, cancellationToken)
            // handles turning off the consumer when app closes
            .ContinueWith(_ => { }, cancellationToken);
    }

    private async Task StartConsuming(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_channel);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await manager.HandleMessageAsync(
                    ea.RoutingKey,
                    ea.Body.ToArray(),
                    cancellationToken);
                
                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "Failed to process message with routing key {RoutingKey}", ea.RoutingKey);

                await _channel!.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    // we don't want to loop forever if message is bad
                    requeue: false,
                    cancellationToken: cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(
            _settings.Queue,
            false,
            consumer,
            cancellationToken);
    }
}
