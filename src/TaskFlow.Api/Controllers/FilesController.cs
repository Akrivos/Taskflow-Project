// src/TaskFlow.Api/Controllers/FilesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Api.Controllers.Requests;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IBlobService _blobService;

    public FilesController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    /// <summary>
    /// Upload αρχείου σε Azure Blob (Azrite στο τοπικό).
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")] // <-- ΑΠΑΡΑΙΤΗΤΟ για Swagger + IFormFile
    [Authorize] // αν θες public, βγάλ’ το
    public async Task<IActionResult> Upload([FromForm] FileUploadRequest request, CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest("No file provided.");

        await using var stream = request.File.OpenReadStream();

        // Ανάλογα με την υπογραφή του IBlobService σου.
        // Πιο πριν είχες κάτι σαν: UploadAsync(container, stream, contentType, fileName, ct)
        await _blobService.UploadAsync(
            request.Container,
            stream,
            request.File.ContentType ?? "application/octet-stream",
            request.File.FileName,
            ct
        );

        return Ok(new { message = "Uploaded", fileName = request.File.FileName, container = request.Container });
    }

    /// <summary>
    /// Διαγραφή blob.
    /// </summary>
    [HttpDelete("{container}/{blobName}")]
    [Authorize] // αν θες public, βγάλ’ το
    public async Task<IActionResult> Delete(string container, string blobName, CancellationToken ct)
    {
        await _blobService.DeleteAsync(container, blobName, ct);
        return NoContent();
    }
}
