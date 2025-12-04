using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Files;

public sealed class FileUploadService : IFileUploadService
{
    public const long MaxFileSizeBytes = 20 * 1024 * 1024;

    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/png"
    };

    private readonly IBlobService _blobService;
    private readonly ICurrentUser _user;

    public FileUploadService(IBlobService blobService, ICurrentUser user)
    {
        _blobService = blobService;
        _user = user;
    }

    public async Task<FileUploadResult> UploadAsync(
        string container,
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct)
    {
        if (fileStream is null || !fileStream.CanRead)
            return FileUploadResult.Fail("Invalid file-stream");

        if (string.IsNullOrWhiteSpace(container))
            return FileUploadResult.Fail("Container is empty.");

        if (string.IsNullOrWhiteSpace(contentType))
            contentType = "application/octet-stream";

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            return FileUploadResult.Fail("Invalid file type");

        var containerName = SanitizeContainerName(container);
        if (string.IsNullOrWhiteSpace(containerName))
            return FileUploadResult.Fail("Invalid container name.");

        var extension = Path.GetExtension(originalFileName);
        var blobName = $"{Guid.NewGuid():N}{extension}";

        var uploadedBy = _user.UserId ?? "anonymous";

        var metadata = new Dictionary<string, string>
        {
            ["originalFileName"] = originalFileName,
            ["uploadedBy"] = uploadedBy,
            ["uploadedAtUtc"] = DateTime.UtcNow.ToString("O")
        };

        var url = await _blobService.UploadAsync(
            containerName,
            fileStream,
            blobName,
            contentType,
            metadata,
            ct);

        return FileUploadResult.Ok(url, containerName, blobName);
    }

    private static string SanitizeContainerName(string input)
        => input.Trim().ToLowerInvariant();
}
