using Microsoft.AspNetCore.Http;

namespace DataShare.Api.Tests.Helpers;

public sealed class TestFormFile : IFormFile
{
    private readonly byte[] _content;

    public TestFormFile(string fileName, string content, string contentType = "text/plain", long? length = null)
    {
        FileName = fileName;
        Name = "file";
        ContentType = contentType;
        _content = System.Text.Encoding.UTF8.GetBytes(content);
        Length = length ?? _content.Length;
    }

    public string ContentType { get; }
    public string ContentDisposition { get; set; } = string.Empty;
    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
    public long Length { get; }
    public string Name { get; }
    public string FileName { get; }

    public void CopyTo(Stream target)
    {
        target.Write(_content, 0, _content.Length);
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        return target.WriteAsync(_content, cancellationToken).AsTask();
    }

    public Stream OpenReadStream()
    {
        return new MemoryStream(_content);
    }
}
