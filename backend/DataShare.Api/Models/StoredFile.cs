namespace DataShare.Api.Models;

public class StoredFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OriginalName { get; set; } = string.Empty;
    public string StoredName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}
