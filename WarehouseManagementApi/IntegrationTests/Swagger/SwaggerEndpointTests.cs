using System.Net;
using FluentAssertions;
using IntegrationTests.Helpers.Dependencies;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests.Swagger;

[Collection("Warehouse API")]
public class SwaggerEndpointTests(CustomWebApplicationFactory factory)
{
    [Fact]
    public async Task Swagger_Json_Endpoint_Returns_Success()
    {
        // factory works in testing environment, so we have to use
        // development environment here only so swagger is created
        await using var devFactory = factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Development"));
        using var devClient = devFactory.CreateClient();

        var response = await devClient.GetAsync("/swagger/v1/swagger.json", CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);
        body.Should().NotBeNullOrWhiteSpace();
    }
}
