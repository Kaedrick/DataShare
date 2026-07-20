using System.Data.Common;
using DataShare.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DataShare.Api.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly string _uploadDirectory = Path.Combine(Path.GetTempPath(), $"datashare-tests-{Guid.NewGuid():N}");

    public CustomWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "32qs1dq3d1q35qs1df3qs13q2s56dq2j",
                ["Jwt:Issuer"] = "DataShare.Api",
                ["Jwt:Audience"] = "DataShare.Client",
                ["Jwt:ExpiresMinutes"] = "120",
                ["Storage:UploadPath"] = _uploadDirectory,
                ["Frontend:PublicBaseUrl"] = "http://localhost:5173"
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbConnection>();
            services.AddSingleton<DbConnection>(_connection);
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
        if (Directory.Exists(_uploadDirectory))
        {
            Directory.Delete(_uploadDirectory, true);
        }
    }
}
