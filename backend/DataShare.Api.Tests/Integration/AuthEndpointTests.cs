using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DataShare.Api.Tests.Infrastructure;

namespace DataShare.Api.Tests.Integration;

public class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidPayload_ReturnsTokenAndEmail()
    {
        var email = $"register.{Guid.NewGuid():N}@example.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!" });

        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(email, document.RootElement.GetProperty("email").GetString());
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var email = $"duplicate.{Guid.NewGuid():N}@example.com";
        await _client.RegisterAndGetTokenAsync(email);

        var response = await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "Password123!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var email = $"login.{Guid.NewGuid():N}@example.com";
        await _client.RegisterAndGetTokenAsync(email, "Password123!");

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Password123!" });

        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var email = $"wrong.{Guid.NewGuid():N}@example.com";
        await _client.RegisterAndGetTokenAsync(email, "Password123!");

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "BadPassword123!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
