namespace DataShare.Api.Services;

public class StorageService
{
    private readonly string _uploadPath;

    public StorageService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var folder = configuration["Storage:UploadPath"] ?? "uploads";
        _uploadPath = Path.IsPathRooted(folder) ? folder : Path.Combine(environment.ContentRootPath, folder);
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<(string storedName, string relativePath)> SaveAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(_uploadPath, storedName);

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream);

        return (storedName, Path.Combine("uploads", storedName));
    }

    public Stream OpenRead(string relativePath)
    {
        return File.OpenRead(ToFullPath(relativePath));
    }

    public void Delete(string relativePath)
    {
        var fullPath = ToFullPath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string ToFullPath(string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            return relativePath;
        }

        return Path.Combine(_uploadPath, Path.GetFileName(relativePath));
    }
}
