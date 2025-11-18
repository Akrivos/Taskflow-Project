namespace TaskFlow.Application.Common.Models;

public sealed class FileUploadResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }

    public string? Url { get; }
    public string? Container { get; }
    public string? BlobName { get; }

    private FileUploadResult(
        bool success,
        string? errorMessage,
        string? url,
        string? container,
        string? blobName)
    {
        Success = success;
        ErrorMessage = errorMessage;
        Url = url;
        Container = container;
        BlobName = blobName;
    }

    public static FileUploadResult Fail(string errorMessage)
        => new(false, errorMessage, null, null, null);

    public static FileUploadResult Ok(string url, string container, string blobName)
        => new(true, null, url, container, blobName);
}
