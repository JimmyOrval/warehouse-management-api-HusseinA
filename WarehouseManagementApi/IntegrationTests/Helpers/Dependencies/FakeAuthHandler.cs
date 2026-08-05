using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Helpers.Dependencies;

public class FakeAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    // used to inject the role claim
    public const string RoleHeader = "Test-Role";
    public const string SchemeName = "Test";
    public const string NoAuthHeader = "Test-NoAuth";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var role = Request.Headers.TryGetValue(RoleHeader, out var value)
        // in case user is normal user
            ? value.ToString()
        // otherwise default to admin for testing only
            : "Admin";

        var claims = new[]
        {
            // custom claims for id adn role
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim("email", "test-user@email.com"),
            new Claim("role", role)
        };
        var identity = new ClaimsIdentity(
            claims,
            SchemeName,
            ClaimTypes.NameIdentifier,
            "role");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}