using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Controllers.Requests;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Files;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly IBlobService _blobService;

    private const long MaxFileSizeBytes = FileUploadService.MaxFileSizeBytes;

    public FilesController(
        IFileUploadService fileUploadService,
        IBlobService blobService)
    {
        _fileUploadService = fileUploadService;
        _blobService = blobService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [Authorize]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Upload(
        [FromForm] FileUploadRequest request,
        CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { error = "File does not exists or its empty." });

        if (request.File.Length > MaxFileSizeBytes)
            return BadRequest(new { error = $"File exceeds maximum size {MaxFileSizeBytes / (1024 * 1024)} MB." });

        var contentType = request.File.ContentType ?? "application/octet-stream";

        await using var stream = request.File.OpenReadStream();

        var result = await _fileUploadService.UploadAsync(
            request.Container,
            stream,
            request.File.FileName,
            contentType,
            ct);

        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Created(result.Url!, new
        {
            message = "Uploaded",
            container = result.Container,
            fileName = result.BlobName,
            url = result.Url
        });
    }

    [HttpDelete("{container}/{blobName}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        string container,
        string blobName,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(container))
            return BadRequest("Invalid container.");

        if (string.IsNullOrWhiteSpace(blobName))
            return BadRequest("Invalid blob.");

        var safeContainer = container.Trim().ToLowerInvariant();
        var safeBlobName = System.IO.Path.GetFileName(blobName);

        var deleted = await _blobService.DeleteAsync(
            safeContainer,
            safeBlobName,
            ct);

        if (!deleted)
            return NotFound(new { error = "Blob has not found." });

        return NoContent();
    }
}
