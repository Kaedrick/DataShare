using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DataShare.Api.Tests.Infrastructure;

namespace DataShare.Api.Tests.Integration;

public class FileEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FileEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var form = CreateUploadForm("blocked.txt", "contenu");
        var response = await _client.PostAsync("/api/files/upload", form);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Upload_WithValidFile_ReturnsDownloadLinkAndMetadata()
    {
        await AuthenticateAsync();
        using var form = CreateUploadForm("document.txt", "contenu de test");

        var uploadResponse = await _client.PostAsync("/api/files/upload", form);
        uploadResponse.EnsureSuccessStatusCode();

        using var uploadDocument = JsonDocument.Parse(await uploadResponse.Content.ReadAsStringAsync());
        var downloadUrl = uploadDocument.RootElement.GetProperty("downloadUrl").GetString();
        var token = downloadUrl!.Split('/').Last();

        Assert.StartsWith("http://localhost:5173/download/", downloadUrl);

        var metadataResponse = await _client.GetAsync($"/api/download/{token}");
        metadataResponse.EnsureSuccessStatusCode();

        using var metadataDocument = JsonDocument.Parse(await metadataResponse.Content.ReadAsStringAsync());
        Assert.Equal("document.txt", metadataDocument.RootElement.GetProperty("originalName").GetString());
        Assert.False(metadataDocument.RootElement.GetProperty("isExpired").GetBoolean());
    }

    [Fact]
    public async Task Download_WithValidToken_ReturnsOriginalFileContent()
    {
        await AuthenticateAsync();
        var token = await UploadAndGetTokenAsync("download.txt", "contenu téléchargeable");

        var response = await _client.PostAsJsonAsync($"/api/download/{token}", new { password = (string?)null });

        response.EnsureSuccessStatusCode();
        Assert.Equal("contenu téléchargeable", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Download_WithProtectedFileAndWrongPassword_ReturnsUnauthorized()
    {
        await AuthenticateAsync();
        var token = await UploadAndGetTokenAsync("protected.txt", "secret", "secret123");

        var response = await _client.PostAsJsonAsync($"/api/download/{token}", new { password = "bad123" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Download_WithProtectedFileAndValidPassword_ReturnsFile()
    {
        await AuthenticateAsync();
        var token = await UploadAndGetTokenAsync("protected-ok.txt", "secret ok", "secret123");

        var response = await _client.PostAsJsonAsync($"/api/download/{token}", new { password = "secret123" });

        response.EnsureSuccessStatusCode();
        Assert.Equal("secret ok", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetMine_ReturnsOnlyAuthenticatedUserFiles()
    {
        await AuthenticateAsync();
        await UploadAndGetTokenAsync("mine.txt", "content");

        var response = await _client.GetAsync("/api/files/me");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("mine.txt", json);
    }

    [Fact]
    public async Task Delete_AsOwner_RemovesFileAndMetadata()
    {
        await AuthenticateAsync();
        var upload = await UploadAndGetPayloadAsync("delete-me.txt", "delete content");
        var id = upload.RootElement.GetProperty("id").GetGuid();
        var token = upload.RootElement.GetProperty("downloadUrl").GetString()!.Split('/').Last();

        var deleteResponse = await _client.DeleteAsync($"/api/files/{id}");
        var metadataResponse = await _client.GetAsync($"/api/download/{token}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, metadataResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_AsDifferentUser_ReturnsNotFound()
    {
        await AuthenticateAsync();
        var upload = await UploadAndGetPayloadAsync("not-yours.txt", "content");
        var id = upload.RootElement.GetProperty("id").GetGuid();

        _client.DefaultRequestHeaders.Authorization = null;
        await AuthenticateAsync();

        var response = await _client.DeleteAsync($"/api/files/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task AuthenticateAsync()
    {
        var token = await _client.RegisterAndGetTokenAsync();
        _client.UseBearerToken(token);
    }

    private async Task<string> UploadAndGetTokenAsync(string fileName, string content, string? password = null)
    {
        using var document = await UploadAndGetPayloadAsync(fileName, content, password);
        return document.RootElement.GetProperty("downloadUrl").GetString()!.Split('/').Last();
    }

    private async Task<JsonDocument> UploadAndGetPayloadAsync(string fileName, string content, string? password = null)
    {
        using var form = CreateUploadForm(fileName, content, password);
        var response = await _client.PostAsync("/api/files/upload", form);
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }

    private static MultipartFormDataContent CreateUploadForm(string fileName, string content, string? password = null)
    {
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        form.Add(fileContent, "file", fileName);
        form.Add(new StringContent("7"), "expirationDays");
        if (password is not null)
        {
            form.Add(new StringContent(password), "password");
        }
        return form;
    }
}
