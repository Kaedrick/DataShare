using DataShare.Api.DTOs;
using DataShare.Api.Services;
using DataShare.Api.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DataShare.Api.Tests.Unit;

public class AuthServiceAdditionalTests
{
    [Fact]
    public async Task RegisterAsync_WithDuplicateEmailDifferentCase_ThrowsInvalidOperationException()
    {
        await using var context = TestDatabase.Create();
        var service = new AuthService(context, TestConfiguration.Create());
        await service.RegisterAsync(new RegisterRequest { Email = "USER@example.com", Password = "Password123!" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(new RegisterRequest { Email = "user@EXAMPLE.com", Password = "Password123!" }));
    }

    [Fact]
    public async Task LoginAsync_TrimsAndNormalizesEmail_ReturnsRegisteredEmail()
    {
        await using var context = TestDatabase.Create();
        var service = new AuthService(context, TestConfiguration.Create());
        await service.RegisterAsync(new RegisterRequest { Email = "User@Example.com", Password = "Password123!" });

        var response = await service.LoginAsync(new LoginRequest { Email = " user@example.com ", Password = "Password123!" });

        Assert.Equal("user@example.com", response.Email);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ThrowsUnauthorizedAccessException()
    {
        await using var context = TestDatabase.Create();
        var service = new AuthService(context, TestConfiguration.Create());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginRequest { Email = "missing@example.com", Password = "Password123!" }));
    }

    [Fact]
    public async Task RegisterAsync_CreatesOneUserOnly()
    {
        await using var context = TestDatabase.Create();
        var service = new AuthService(context, TestConfiguration.Create());

        await service.RegisterAsync(new RegisterRequest { Email = "new@example.com", Password = "Password123!" });

        Assert.Equal(1, await context.Users.CountAsync());
    }
}
