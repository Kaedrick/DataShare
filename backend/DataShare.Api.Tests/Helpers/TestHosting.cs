using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace DataShare.Api.Tests.Helpers;

public sealed class TestWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "DataShare.Api.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Testing";
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}

public static class TestConfiguration
{
    public static IConfiguration Create(string? uploadPath = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "32qs1dq3d1q35qs1df3qs13q2s56dq2j",
                ["Jwt:Issuer"] = "DataShare.Api",
                ["Jwt:Audience"] = "DataShare.Client",
                ["Jwt:ExpiresMinutes"] = "120",
                ["Storage:UploadPath"] = uploadPath ?? "uploads"
            })
            .Build();
    }
}
