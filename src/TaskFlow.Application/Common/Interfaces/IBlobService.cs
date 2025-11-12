namespace TaskFlow.Application.Common.Interfaces;
public interface IBlobService
{
    Task<string> UploadAsync(string containerName, Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string containerName, string fileName, CancellationToken ct = default);
}
