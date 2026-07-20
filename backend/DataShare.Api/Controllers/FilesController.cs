using System.Security.Claims;
using DataShare.Api.DTOs;
using DataShare.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataShare.Api.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly IConfiguration _configuration;

    public FilesController(FileService fileService, IConfiguration configuration)
    {
        _fileService = fileService;
        _configuration = configuration;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(1024L * 1024L * 1024L)]
    public async Task<ActionResult<UploadResponse>> Upload([FromForm] IFormFile file, [FromForm] int expirationDays = 7, [FromForm] string? password = null)
    {
        try
        {
            return Ok(await _fileService.UploadAsync(GetUserId(), file, expirationDays, password, GetPublicBaseUrl()));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<FileItemResponse>>> GetMine()
    {
        return Ok(await _fileService.GetUserFilesAsync(GetUserId(), GetPublicBaseUrl()));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _fileService.DeleteAsync(GetUserId(), id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(value!);
    }

    private string GetPublicBaseUrl()
    {
        return _configuration["Frontend:PublicBaseUrl"]?.TrimEnd('/') ?? $"{Request.Scheme}://{Request.Host}";
    }
}
