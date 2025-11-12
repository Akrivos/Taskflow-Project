using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Files;

public sealed class AzureBlobService : IBlobService
{
    private readonly BlobServiceClient _client;

    public AzureBlobService(BlobServiceClient client)
    {
        _client = client;
    }

    public async Task<string> UploadAsync(string containerName, Stream stream, string fileName, string contentType, CancellationToken ct)
    {
        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(fileName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blob.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, ct);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string containerName, string blobName, CancellationToken ct)
    {
        var container = _client.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
    }
}

