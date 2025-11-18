namespace TaskFlow.Infrastructure.Files;

public sealed class BlobRetryOptions
{
    public int RetryCount { get; set; } = 5;
    public int BaseDelaySeconds { get; set; } = 1;
    public int MaxDelaySeconds { get; set; } = 15;
}