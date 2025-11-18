using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net.Sockets;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Files;

public sealed class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;
    private readonly ILogger<AzureBlobService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AzureBlobService(
        BlobServiceClient client,
        ILogger<AzureBlobService> logger,
        IOptions<BlobRetryOptions> retryOptions)
    {
        _client = client;
        _logger = logger;

        var opts = retryOptions.Value;

        _retryPolicy = Policy
            .Handle<RequestFailedException>(IsTransient)
            .Or<SocketException>()
            .Or<IOException>()
            .Or<TimeoutException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: opts.RetryCount,
                sleepDurationProvider: attempt =>
                {
                    var delay = TimeSpan.FromSeconds(
                        Math.Min(
                            opts.BaseDelaySeconds * Math.Pow(2, attempt),
                            opts.MaxDelaySeconds));

                    return delay;
                },
                onRetry: (exception, delay, attempt, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "[AzureBlob Retry] Attempt {Attempt}/{Max} - retrying in {Delay}s - Correlation: {CorrelationId}",
                        attempt,
                        opts.RetryCount,
                        delay.TotalSeconds,
                        context.TryGetValue("CorrelationId", out var cid) ? cid : "<none>");
                });
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

        return await _retryPolicy.ExecuteAsync(async (ctx, innerCt) =>
        {
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

            await blob.UploadAsync(stream, options, innerCt);

            _logger.LogInformation(
                "[AzureBlob Upload] CorrelationId: {CorrelationId} - Upload completed. Uri: {Uri}",
                correlationId, blob.Uri);

            return blob.Uri.ToString();

        }, new Context { { "CorrelationId", correlationId } }, ct);
    }

    public async Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken ct = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        var container = _client.GetBlobContainerClient(containerName);

        return await _retryPolicy.ExecuteAsync(async (ctx, innerCt) =>
        {
            var blob = container.GetBlobClient(blobName);

            _logger.LogInformation(
                "[AzureBlob Delete] CorrelationId: {CorrelationId} - Attempting delete: {Blob}",
                correlationId, blobName);

            var result = await blob.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: innerCt);

            _logger.LogInformation(
                "[AzureBlob Delete] CorrelationId: {CorrelationId} - Deleted: {Deleted}",
                correlationId, result.Value);

            return result.Value;

        }, new Context { { "CorrelationId", correlationId } }, ct);
    }

    private static bool IsTransient(RequestFailedException ex)
        => ex.Status is 408 or 500 or 502 or 503 or 504;
}
