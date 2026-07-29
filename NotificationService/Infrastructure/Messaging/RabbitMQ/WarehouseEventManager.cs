using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Domain.Interfaces;
using Infrastructure.Messaging.Contracts;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.RabbitMQ;

public class WarehouseEventManager(
    IServiceScopeFactory scopeFactory,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<WarehouseEventManager> logger) : IWarehouseEventManager
{
    public async Task HandleMessageAsync(
        string routingKey,
        byte[] body,
        CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(body);

        using var scope = scopeFactory.CreateScope();
        var ingestionService = scope.ServiceProvider.GetRequiredService<INotificationConsumptionService>();

        switch (routingKey)
        {
            case "stock.low":
                var stockLow = JsonSerializer.Deserialize<StockLowDetected>(json, jsonSerializerOptions)
                               ?? throw new ValidationException("Serialization failed.");
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(stockLow), CancellationToken.None);
                break;

            case "file.uploaded":
                var fileUploaded = JsonSerializer.Deserialize<WarehouseFileUploaded>(json, jsonSerializerOptions)
                                   ?? throw new ValidationException("Serialization failed.");
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(fileUploaded), CancellationToken.None);
                break;

            case "stock.adjusted":
                var stockAdjusted = JsonSerializer.Deserialize<StockAdjusted>(json, jsonSerializerOptions)
                                    ?? throw new ValidationException("Serialization failed.");
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(stockAdjusted), CancellationToken.None);
                break;

            case "product.created":
                var productCreated = JsonSerializer.Deserialize<ProductCreated>(json, jsonSerializerOptions)
                                     ?? throw new ValidationException("Serialization failed.");
                await ingestionService.ConsumeAsync(WarehouseEventMapper.Map(productCreated), CancellationToken.None);
                break;

            default:
                logger.LogWarning("Received message with unrecognized routing key {RoutingKey}", routingKey);
                break;
        }
    }
}