using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.IntegrationTests.Infrastructure
{
    public class FakeQueueService : IQueueService
    {
        public Task PublishAsync(string topic, string message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
