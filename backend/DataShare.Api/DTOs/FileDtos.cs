namespace DataShare.Api.DTOs;

public record FileItemResponse(
    Guid Id,
    string OriginalName,
    long Size,
    DateTime UploadedAt,
    DateTime ExpiresAt,
    bool IsExpired,
    string DownloadUrl
);

public record UploadResponse(
    Guid Id,
    string OriginalName,
    long Size,
    DateTime ExpiresAt,
    string DownloadUrl
);

public record DownloadMetadataResponse(
    string OriginalName,
    long Size,
    DateTime ExpiresAt,
    bool IsExpired,
    bool RequiresPassword
);

public class DownloadRequest
{
    public string? Password { get; set; }
}
