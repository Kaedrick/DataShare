using System.Security.Cryptography;
using DataShare.Api.Data;
using DataShare.Api.DTOs;
using DataShare.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DataShare.Api.Services;

public class FileService
{
    private const long MaxSize = 1024L * 1024L * 1024L;
    private static readonly string[] ForbiddenExtensions = [".exe", ".bat", ".cmd", ".sh", ".js"];

    private readonly AppDbContext _db;
    private readonly StorageService _storage;

    public FileService(AppDbContext db, StorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<UploadResponse> UploadAsync(Guid userId, IFormFile file, int expirationDays, string? password, string baseUrl)
    {
        if (file.Length <= 0) throw new InvalidOperationException("Le fichier est vide.");
        if (file.Length > MaxSize) throw new InvalidOperationException("La taille maximale est de 1 Go.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ForbiddenExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Ce type de fichier n'est pas autorisé.");
        }

        if (expirationDays < 1 || expirationDays > 7)
        {
            throw new InvalidOperationException("La durée d'expiration doit être comprise entre 1 et 7 jours.");
        }

        if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
        {
            throw new InvalidOperationException("Le mot de passe doit contenir au moins 6 caractères.");
        }

        var saved = await _storage.SaveAsync(file);
        var token = CreateToken();

        var storedFile = new StoredFile
        {
            OriginalName = file.FileName,
            StoredName = saved.storedName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            Size = file.Length,
            RelativePath = saved.relativePath,
            Token = token,
            PasswordHash = string.IsNullOrWhiteSpace(password) ? null : BCrypt.Net.BCrypt.HashPassword(password),
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            UserId = userId
        };

        _db.StoredFiles.Add(storedFile);
        await _db.SaveChangesAsync();

        return new UploadResponse(storedFile.Id, storedFile.OriginalName, storedFile.Size, storedFile.ExpiresAt, $"{baseUrl}/download/{token}");
    }
    public async Task<List<FileItemResponse>> GetUserFilesAsync(Guid userId, string baseUrl)
    {
        return await _db.StoredFiles
            .Where(file => file.UserId == userId)
            .OrderByDescending(file => file.UploadedAt)
            .Select(file => new FileItemResponse(
                file.Id,
                file.OriginalName,
                file.Size,
                file.UploadedAt,
                file.ExpiresAt,
                DateTime.UtcNow > file.ExpiresAt,
                $"{baseUrl}/download/{file.Token}"
            ))
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid userId, Guid fileId)
    {
        var file = await _db.StoredFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
        if (file is null) throw new KeyNotFoundException("Fichier introuvable.");

        _storage.Delete(file.RelativePath);
        _db.StoredFiles.Remove(file);
        await _db.SaveChangesAsync();
    }

    public async Task<DownloadMetadataResponse> GetDownloadMetadataAsync(string token)
    {
        var file = await _db.StoredFiles.FirstOrDefaultAsync(f => f.Token == token);
        if (file is null) throw new KeyNotFoundException("Lien invalide.");

        return new DownloadMetadataResponse(file.OriginalName, file.Size, file.ExpiresAt, file.IsExpired, file.PasswordHash is not null);
    }

    public async Task<(StoredFile file, Stream stream)> PrepareDownloadAsync(string token, string? password)
    {
        var file = await _db.StoredFiles.FirstOrDefaultAsync(f => f.Token == token);
        if (file is null) throw new KeyNotFoundException("Lien invalide.");
        if (file.IsExpired) throw new InvalidOperationException("Ce fichier a expiré.");

        if (file.PasswordHash is not null)
        {
            if (string.IsNullOrWhiteSpace(password) || !BCrypt.Net.BCrypt.Verify(password, file.PasswordHash))
            {
                throw new UnauthorizedAccessException("Mot de passe incorrect.");
            }
        }

        return (file, _storage.OpenRead(file.RelativePath));
    }

    private static string CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
