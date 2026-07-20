using DataShare.Api.Data;
using DataShare.Api.DTOs;
using DataShare.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataShare.Api.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_StoresLowercaseEmailAndHashedPassword()
    {
        await using var db = CreateDbContext();
        var service = new AuthService(db, CreateConfiguration());

        await service.RegisterAsync(new RegisterRequest { Email = "TEST@EXAMPLE.COM", Password = "Password123!" });

        var user = await db.Users.SingleAsync();
        Assert.Equal("test@example.com", user.Email);
        Assert.NotEqual("Password123!", user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        await using var db = CreateDbContext();
        var service = new AuthService(db, CreateConfiguration());
        await service.RegisterAsync(new RegisterRequest { Email = "test@example.com", Password = "Password123!" });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "wrong" }));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "32qs1dq3d1q35qs1df3qs13q2s56dq2j",
                ["Jwt:Issuer"] = "DataShare.Api",
                ["Jwt:Audience"] = "DataShare.Client",
                ["Jwt:ExpiresMinutes"] = "120"
            })
            .Build();
    }
}
