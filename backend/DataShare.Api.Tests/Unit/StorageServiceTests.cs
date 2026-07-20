using DataShare.Api.Services;
using DataShare.Api.Tests.Helpers;

namespace DataShare.Api.Tests.Unit;

public class StorageServiceTests
{
    [Fact]
    public async Task SaveAsync_CreatesFileInUploadDirectory()
    {
        using var environment = new TemporaryDirectory();
        var service = CreateService(environment.Root);
        var file = new TestFormFile("sample.txt", "storage content");

        var saved = await service.SaveAsync(file);
        var physicalPath = Path.Combine(environment.Root, saved.relativePath);

        Assert.True(File.Exists(physicalPath));
        Assert.Equal("storage content", await File.ReadAllTextAsync(physicalPath));
        Assert.StartsWith("uploads", saved.relativePath);
    }

    [Fact]
    public async Task OpenRead_ReturnsSavedFileStream()
    {
        using var environment = new TemporaryDirectory();
        var service = CreateService(environment.Root);
        var file = new TestFormFile("read.txt", "readable");
        var saved = await service.SaveAsync(file);

        await using var stream = service.OpenRead(saved.relativePath);
        using var reader = new StreamReader(stream);

        Assert.Equal("readable", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task Delete_RemovesExistingFile()
    {
        using var environment = new TemporaryDirectory();
        var service = CreateService(environment.Root);
        var file = new TestFormFile("delete.txt", "delete me");
        var saved = await service.SaveAsync(file);
        var physicalPath = Path.Combine(environment.Root, saved.relativePath);

        service.Delete(saved.relativePath);

        Assert.False(File.Exists(physicalPath));
    }

    [Fact]
    public void Delete_WithMissingFile_DoesNotThrow()
    {
        using var environment = new TemporaryDirectory();
        var service = CreateService(environment.Root);

        var exception = Record.Exception(() => service.Delete(Path.Combine("uploads", "missing.txt")));

        Assert.Null(exception);
    }

    private static StorageService CreateService(string root)
    {
        return new StorageService(TestConfiguration.Create("uploads"), new TestWebHostEnvironment { ContentRootPath = root });
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        private readonly string _previousDirectory;
        public string Root { get; } = Path.Combine(Path.GetTempPath(), $"datashare-storage-{Guid.NewGuid():N}");

        public TemporaryDirectory()
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
