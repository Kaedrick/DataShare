using DataShare.Api.DTOs;
using DataShare.Api.Models;
using DataShare.Api.Services;
using DataShare.Api.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using DataShare.Api.Data;

namespace DataShare.Api.Tests.Unit;

public class FileServiceAdditionalTests
{
    [Fact]
    public async Task UploadAsync_WithEmptyFile_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("empty.txt", string.Empty, length: 0);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 7, null, "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithTooLargeFile_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("large.txt", "x", length: 1024L * 1024L * 1024L + 1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 7, null, "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithForbiddenExtension_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("virus.exe", "x");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 7, null, "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithExpirationBelowMinimum_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("file.txt", "x");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 0, null, "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithExpirationAboveMaximum_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("file.txt", "x");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 8, null, "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithShortPassword_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var file = new TestFormFile("file.txt", "x");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadAsync(Guid.NewGuid(), file, 7, "123", "http://localhost"));
    }

    [Fact]
    public async Task UploadAsync_WithValidFileAndPassword_CreatesMetadataAndPasswordHash()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var userId = Guid.NewGuid();
        var file = new TestFormFile("report.txt", "hello", string.Empty);

        var response = await service.UploadAsync(userId, file, 3, "secret123", "http://front.test");
        var storedFile = await context.StoredFiles.SingleAsync();

        Assert.Equal("report.txt", response.OriginalName);
        Assert.Equal("report.txt", storedFile.OriginalName);
        Assert.Equal("application/octet-stream", storedFile.ContentType);
        Assert.Equal(userId, storedFile.UserId);
        Assert.NotNull(storedFile.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("secret123", storedFile.PasswordHash));
        Assert.Contains("/download/", response.DownloadUrl);
    }

    [Fact]
    public async Task GetUserFilesAsync_ReturnsOnlyFilesForRequestedUser()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        context.StoredFiles.Add(CreateStoredFile(userId, "mine.txt", "token-1", DateTime.UtcNow.AddDays(1)));
        context.StoredFiles.Add(CreateStoredFile(otherUserId, "other.txt", "token-2", DateTime.UtcNow.AddDays(1)));
        await context.SaveChangesAsync();

        var files = await service.GetUserFilesAsync(userId, "http://localhost");

        Assert.Single(files);
        Assert.Equal("mine.txt", files[0].OriginalName);
        Assert.False(files[0].IsExpired);
        Assert.Contains("token-1", files[0].DownloadUrl);
    }

    [Fact]
    public async Task GetDownloadMetadataAsync_WithInvalidToken_ThrowsKeyNotFoundException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDownloadMetadataAsync("missing"));
    }

    [Fact]
    public async Task GetDownloadMetadataAsync_WithProtectedExpiredFile_ReturnsFlags()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        context.StoredFiles.Add(CreateStoredFile(Guid.NewGuid(), "old.txt", "old-token", DateTime.UtcNow.AddDays(-1), BCrypt.Net.BCrypt.HashPassword("secret123")));
        await context.SaveChangesAsync();

        var metadata = await service.GetDownloadMetadataAsync("old-token");

        Assert.Equal("old.txt", metadata.OriginalName);
        Assert.True(metadata.IsExpired);
        Assert.True(metadata.RequiresPassword);
    }

    [Fact]
    public async Task PrepareDownloadAsync_WithInvalidToken_ThrowsKeyNotFoundException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.PrepareDownloadAsync("missing", null));
    }

    [Fact]
    public async Task PrepareDownloadAsync_WithExpiredFile_ThrowsInvalidOperationException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        context.StoredFiles.Add(CreateStoredFile(Guid.NewGuid(), "expired.txt", "expired-token", DateTime.UtcNow.AddDays(-1)));
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.PrepareDownloadAsync("expired-token", null));
    }

    [Fact]
    public async Task PrepareDownloadAsync_WithProtectedFileWithoutPassword_ThrowsUnauthorizedAccessException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        context.StoredFiles.Add(CreateStoredFile(Guid.NewGuid(), "protected.txt", "protected-token", DateTime.UtcNow.AddDays(1), BCrypt.Net.BCrypt.HashPassword("secret123")));
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.PrepareDownloadAsync("protected-token", null));
    }

    [Fact]
    public async Task DeleteAsync_WithMissingFile_ThrowsKeyNotFoundException()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingFile_RemovesDatabaseRowAndPhysicalFile()
    {
        using var context = TestDatabase.Create();
        using var environment = new TemporaryTestEnvironment();
        var service = CreateService(context, environment.Root);
        var userId = Guid.NewGuid();
        var uploadsPath = Path.Combine(environment.Root, "uploads");
        Directory.CreateDirectory(uploadsPath);
        var physicalPath = Path.Combine(uploadsPath, "stored.txt");
        await File.WriteAllTextAsync(physicalPath, "content");
        var storedFile = CreateStoredFile(userId, "delete.txt", "delete-token", DateTime.UtcNow.AddDays(1));
        storedFile.RelativePath = Path.Combine("uploads", "stored.txt");
        context.StoredFiles.Add(storedFile);
        await context.SaveChangesAsync();

        await service.DeleteAsync(userId, storedFile.Id);

        Assert.False(await context.StoredFiles.AnyAsync());
        Assert.False(File.Exists(physicalPath));
    }

    private static FileService CreateService(AppDbContext context, string root)
    {
        var storage = new StorageService(TestConfiguration.Create("uploads"), new TestWebHostEnvironment { ContentRootPath = root });
        return new FileService(context, storage);
    }

    private static StoredFile CreateStoredFile(Guid userId, string name, string token, DateTime expiresAt, string? passwordHash = null)
    {
        return new StoredFile
        {
            Id = Guid.NewGuid(),
            OriginalName = name,
            StoredName = $"{Guid.NewGuid():N}.txt",
            ContentType = "text/plain",
            Size = 12,
            RelativePath = Path.Combine("uploads", $"{Guid.NewGuid():N}.txt"),
            Token = token,
            PasswordHash = passwordHash,
            UploadedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            UserId = userId
        };
    }

    private sealed class TemporaryTestEnvironment : IDisposable
    {
        private readonly string _previousDirectory;
        public string Root { get; } = Path.Combine(Path.GetTempPath(), $"datashare-unit-{Guid.NewGuid():N}");

        public TemporaryTestEnvironment()
        {
            Directory.CreateDirectory(Root);
            _previousDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Root);
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_previousDirectory);
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }
    }
}
