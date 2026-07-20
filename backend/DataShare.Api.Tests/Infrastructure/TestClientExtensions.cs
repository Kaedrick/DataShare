using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DataShare.Api.Tests.Infrastructure;

public static class TestClientExtensions
{
    public static async Task<string> RegisterAndGetTokenAsync(this HttpClient client, string? email = null, string password = "Password123!")
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = email ?? $"user.{Guid.NewGuid():N}@example.com",
            password
        });

        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("token").GetString()!;
    }

    public static void UseBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
