using Domain.Interfaces;
using Hangfire.MemoryStorage;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Minio;
using Moq;
using StackExchange.Redis;
using HangfireServiceCollectionExtensions = Hangfire.HangfireServiceCollectionExtensions;

namespace IntegrationTests.Helpers.Dependencies;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // replace read DB with fake one
            services.RemoveAll<DbContextOptions<WarehouseDbContext>>();
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // replace redis
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
            
            // replace redis connection to in-memory storage
            services.RemoveAll<IConnectionMultiplexer>();
            services.AddSingleton(CreateFakeMultiplexer());
            
            // replace rabbitmq with fake publisher
            services.RemoveAll<IEventPublisher>();
            services.AddSingleton<IEventPublisher, FakeEventPublisher>();
            
            // replace minio
            services.RemoveAll<IFileStorageService>();
            services.AddSingleton<IFileStorageService, InMemoryFileStorageService>();
            services.RemoveAll<MinioClient>();

            // let hangfire use in-memory storage
            HangfireServiceCollectionExtensions.AddHangfire(services, config => GlobalConfigurationExtensions.UseMemoryStorage(config));

            // replace firebase auth with our fake scheme
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = FakeAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = FakeAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions,
                    FakeAuthHandler>(FakeAuthHandler.SchemeName,
                    _ => { });
        });
    }

    // this creates a fake in-memory storage to replace redis
    private static IConnectionMultiplexer CreateFakeMultiplexer()
    {
        var mock = new Mock<IConnectionMultiplexer>();
                mock.Setup(m => m.IsConnected).Returns(true);
                mock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                    .Returns(new Mock<IDatabase>().Object);
        return mock.Object;
    }
}