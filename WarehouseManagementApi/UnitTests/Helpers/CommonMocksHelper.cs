using Application.Common;
using AutoMapper;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Helpers;

public class CommonMocksHelper
{
    // to create a mock mapper
    public static IMapper MockMapper(Action<IMapperConfigurationExpression> configuration)
    {
        var config = new MapperConfiguration(
            configuration,
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    // create a mock event publisher from our real publisher
    public static IEventPublisher MockEventPublisher()
    {
        var publisher = new Mock<IEventPublisher>(MockBehavior.Loose);
        
        // since we only have WarehouseEvent and no other ones
        publisher.Setup(x => x.PublishAsync<WarehouseEvent>(
                It.IsAny<WarehouseEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return publisher.Object;
    }

    // create a cache instance to connect to instead of redis
    public static IDistributedCache MockDistributedCache()
    {
        var options = Options.Create(new MemoryDistributedCacheOptions());
        return new MemoryDistributedCache(options);
    }

    // replaces the normal logger + for any type
    public static ILogger<T> MockLogger<T>()
    {
        return NullLogger<T>.Instance;
    }

    // mock for AI-generated test
    public static Mock<ILogger<T>> MockLoggerMiddleware<T>()
    {
        var logger = new Mock<ILogger<T>>(MockBehavior.Loose);
        return logger;
    }

    public static ICacheStatsTracker MockCacheStatsTracker()
    {
        return new Mock<ICacheStatsTracker>().Object;
    }

    public static IConfiguration MockConfiguration()
    {
        var config = new Dictionary<string, string>
        {
            { "Notifications:MinimumLowQuantity", "20" } 
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(config!)
            .Build();
    }

    public static ICorrelationIdProvider MockCorrelationIdProvider()
    {
        return Mock.Of<ICorrelationIdProvider>(p => p.CorrelationId() == "test-correlation-id");
    }
}