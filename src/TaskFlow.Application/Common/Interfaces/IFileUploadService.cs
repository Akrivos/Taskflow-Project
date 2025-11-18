using System.Security.Claims;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Common.Interfaces;

public interface IFileUploadService
{
    Task<FileUploadResult> UploadAsync(
        string container,
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct);
}
