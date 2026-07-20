using DataShare.Api.DTOs;
using DataShare.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataShare.Api.Controllers;

[ApiController]
[Route("api/download")]
public class DownloadController : ControllerBase
{
    private readonly FileService _fileService;

    public DownloadController(FileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("{token}")]
    public async Task<ActionResult<DownloadMetadataResponse>> Metadata(string token)
    {
        try
        {
            return Ok(await _fileService.GetDownloadMetadataAsync(token));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{token}")]
    public async Task<IActionResult> Download(string token, DownloadRequest request)
    {
        try
        {
            var result = await _fileService.PrepareDownloadAsync(token, request.Password);
            return File(result.stream, result.file.ContentType, result.file.OriginalName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{token}/file")]
    public async Task<IActionResult> DirectDownload(string token)
    {
        try
        {
            var result = await _fileService.PrepareDownloadAsync(token, null);
            return File(result.stream, result.file.ContentType, result.file.OriginalName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
