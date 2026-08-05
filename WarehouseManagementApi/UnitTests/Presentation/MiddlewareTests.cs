using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Presentation.Middleware;

namespace Tests.Presentation;

public class MiddlewareTests
{
    [Fact]
    public async Task SlowRequest_LogsWarning()
    {
        var logger = new Mock<ILogger<SlowRequestLoggingMiddleware>>();

        // create a mock request with code 200
        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                Path = "/products"
            },
            Response =
            {
                StatusCode = StatusCodes.Status200OK
            }
        };

        var middleware = new SlowRequestLoggingMiddleware(Next, logger.Object);

        await middleware.Invoke(context);

        logger.Verify(
            r => r.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    // check if the middleware caught our values
                    o.ToString()!.Contains("/products") &&
                    o.ToString()!.Contains("200")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        return;

        // custom delay to trigger the 5 second delay
        async Task Next(HttpContext _)
        {
            await Task.Delay(600);
        }
    }
}