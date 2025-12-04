using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Files;

public sealed class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;
    private readonly ILogger<AzureBlobService> _logger;

    public AzureBlobService(
        BlobServiceClient client,
        ILogger<AzureBlobService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<string> UploadAsync(
        string containerName,
        Stream stream,
        string blobName,
        string contentType,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType };

        var options = new BlobUploadOptions
        {
            HttpHeaders = headers,
            Metadata = metadata
        };

        _logger.LogInformation(
            "[AzureBlob Upload] CorrelationId: {CorrelationId} - Uploading blob '{Blob}' to container '{Container}'",
            correlationId, blobName, containerName);

        await blob.UploadAsync(stream, options, ct);

        _logger.LogInformation(
            "[AzureBlob Upload] CorrelationId: {CorrelationId} - Upload completed. Uri: {Uri}",
            correlationId, blob.Uri);

        return blob.Uri.ToString();
    }

    public async Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken ct = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);

        _logger.LogInformation(
            "[AzureBlob Delete] CorrelationId: {CorrelationId} - Attempting dlete: {Blob}",
            correlationId, blobName);

        var result = await blob.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: ct);

        _logger.LogInformation(
            "[AzureBlob Delete] CorrelationId: {CorrelationId} - Deleted: {Deleted}",
            correlationId, result.Value);

        return result.Value;
    }
}
