using System.Text;
using System.Text.Json;
using Domain.Interfaces;
using Infrastructure.Messaging.Contracts;
using Infrastructure.Messaging.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Infrastructure.Messaging;

public class RabbitMqConsumer(
    IOptions<RabbitMqSettings> options,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqConsumer> logger) : BackgroundService
{
    private readonly RabbitMqSettings _settings = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    private static readonly JsonSerializerOptions SOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    /*
     * ExecuteAsync creates the connection loop with 5 second retries if broker is off
     * Then it creates exchange + queue and binds them and connects to consumer
     * Then we start consuming the incoming messages. Every message is routed by its routing key
     * Using the key, we know which event was sent to us from the API
     * Successes return an ACK and fails return NACK
     */

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // keep retrying if broker is not running
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Connect(cancellationToken);
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


    private async Task Connect(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await _channel.ExchangeDeclareAsync(
            _settings.Exchange,
            ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);
        
        await _channel.QueueDeclareAsync(
            _settings.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        foreach(var routingKey in _settings.RoutingKeys)
            await _channel.QueueBindAsync(
                _settings.Queue,
                _settings.Exchange,
                routingKey,
                cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            0,
            1,
            false,
            cancellationToken: cancellationToken);
    }

    private async Task StartConsuming(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_channel);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await HandleMessageAsync(ea.RoutingKey, ea.Body.ToArray());
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
    
    private async Task HandleMessageAsync(string routingKey, byte[] body)
    {
        var json = Encoding.UTF8.GetString(body);

        using var scope = scopeFactory.CreateScope();
        var ingestionService = scope.ServiceProvider.GetRequiredService<INotificationConsumptionService>();

        switch (routingKey)
        {
            case "stock.low":
                var stockLow = Deserialize<StockLowDetected>(json);
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(stockLow), CancellationToken.None);
                break;

            case "file.uploaded":
                var fileUploaded = Deserialize<WarehouseFileUploaded>(json);
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(fileUploaded), CancellationToken.None);
                break;

            case "stock.adjusted":
                var stockAdjusted = Deserialize<StockAdjusted>(json);
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(stockAdjusted), CancellationToken.None);
                break;

            case "product.created":
                var productCreated = Deserialize<ProductCreated>(json);
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(productCreated), CancellationToken.None);
                break;

            default:
                logger.LogWarning("Received message with unrecognized routing key {RoutingKey}", routingKey);
                break;
        }
    }
    
    private static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, SOptions)!;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
