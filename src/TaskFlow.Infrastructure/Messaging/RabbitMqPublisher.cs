using System.Text;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Messaging
{
    public sealed class RabbitMqPublisher : IQueueService, IDisposable
    {
        private readonly RabbitMQ.Client.IConnection _connection;

        public RabbitMqPublisher(RabbitMQ.Client.IConnectionFactory factory)
        {
            _connection = factory.CreateConnection();
        }

        public Task PublishAsync(string queue, string message, CancellationToken ct)
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>()
            );

            var body = Encoding.UTF8.GetBytes(message);
            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: props,
                body: body
            );

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            try { _connection?.Close(); } catch { }
            _connection?.Dispose();
        }
    }
}
