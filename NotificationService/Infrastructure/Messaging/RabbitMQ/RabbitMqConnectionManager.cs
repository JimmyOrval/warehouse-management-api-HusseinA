using Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.RabbitMQ;

public class RabbitMqConnectionManager(
    IOptions<RabbitMqSettings> options)
    : IRabbitMqConnectionManager, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;
    
    public async Task<IChannel> ConnectAsync(CancellationToken cancellationToken)
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

        // we first have separate declaration for dead-letter handling
        await _channel.ExchangeDeclareAsync(
            _settings.DeadLetterExchange,
            ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);
        
        
        await _channel.QueueDeclareAsync(
            _settings.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync(
            _settings.DeadLetterQueue,
            _settings.DeadLetterExchange,
            routingKey: "",
            cancellationToken: cancellationToken);

        // queue here sends failed messages to DLX
        var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", _settings.DeadLetterExchange }
        };
        
        await _channel.QueueDeclareAsync(
            _settings.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs!,
            cancellationToken: cancellationToken);

        foreach (var routingKey in _settings.RoutingKeys)
        {
            await _channel.QueueBindAsync(
                _settings.Queue,
                _settings.Exchange,
                routingKey,
                cancellationToken: cancellationToken);
        }

        await _channel.BasicQosAsync(
            0,
            1,
            false,
            cancellationToken: cancellationToken);

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}