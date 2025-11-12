using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TaskFlow.Api.Workers;
public class RabbitMqTaskCreatedConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqTaskCreatedConsumer> _logger;
    private readonly IConfiguration _config;
    private IConnection? _conn;
    private IModel? _channel;

    public RabbitMqTaskCreatedConsumer(ILogger<RabbitMqTaskCreatedConsumer> logger, IConfiguration config)
    {
        _logger = logger; _config = config;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:HostName"] ?? "localhost",
            UserName = _config["RabbitMQ:UserName"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest"
        };
        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare("task-created", durable: true, exclusive: false, autoDelete: false);
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null) return Task.CompletedTask;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var msg = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received from 'task-created': {Message}", msg);
            _channel?.BasicAck(ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume("task-created", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close(); _conn?.Close();
        base.Dispose();
    }
}
