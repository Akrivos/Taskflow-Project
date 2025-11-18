namespace TaskFlow.Application.Common.Interfaces;

public interface IBlobService
{
    Task<string> UploadAsync(
        string containerName,
        Stream stream,
        string blobName,
        string contentType,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken ct = default);
}
