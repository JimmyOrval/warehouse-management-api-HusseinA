using Domain.Enums;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IWebHostEnvironment environment)
    : ControllerBase
{
    // only-admin method to set roles
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("users/{id}/role")]
    public async Task<IActionResult> SetRole([FromRoute] string id, [FromQuery] UserRoles role)
    {
        // outside of development, no one can access these methods
        if (!environment.IsDevelopment())
            return NotFound();
        
        try
        {
            var claims = new Dictionary<string, object>()
            {
                { "role", role.ToString() }
            };
            // firebase needs a key-value to set a claim
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(id, claims);
            return NoContent();
        }

        catch (FirebaseAuthException e)
        {
            return BadRequest(new{ message = "Failed to update user role.", error = e.Message });
        }
    }

    // login method, returns jwt token from firebase
    [HttpPost("auth/token")]
    public async Task<IActionResult> GetToken(
        [FromQuery] string email,
        [FromQuery] string password,
        CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
            return NotFound();
        
        var webApiKey = configuration["Firebase:WebApiKey"];
        var client = httpClientFactory.CreateClient("FirebaseAuth");

        var payload = new
        {
            email,
            password,
            returnSecureToken = true
        };

        var response = await client.PostAsJsonAsync(
            $"/v1/accounts:signInWithPassword?key={webApiKey}",
            payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return StatusCode((int)response.StatusCode, error);
        }
        
        // forms the response to include the token and its expiry time
        var result  = await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>(cancellationToken);
        return Ok(new { idToken = result!.IdToken, expiresIn = result.ExpiresIn });
    }
}

public class FirebaseSignInResponse
{
    public string IdToken { get; init; } = null!;
    public string ExpiresIn { get; init; } = null!;
}