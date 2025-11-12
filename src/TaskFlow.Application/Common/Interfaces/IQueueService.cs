namespace TaskFlow.Application.Common.Interfaces;
public interface IQueueService
{
    Task PublishAsync(string queueName, string payload, CancellationToken ct = default);
}
