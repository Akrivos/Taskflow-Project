using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TaskFlow.Application.Common.Messages;

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
            HostName = _config["RabbitMQ:HostName"],
            UserName = _config["RabbitMQ:UserName"],
            Password = _config["RabbitMQ:Password"]
        };
        _conn = factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare(Topics.TaskCreated, durable: true, exclusive: false, autoDelete: false);
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
            _logger.LogInformation("Received from {Topic}: {Message}", Topics.TaskCreated, msg);
            _channel?.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(Topics.TaskCreated, autoAck: false, consumer: consumer);

        // Κρατάμε ζωντανό το background service μέχρι να ζητηθεί cancellation
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _channel?.Close(); _conn?.Close();
        base.Dispose();
    }
}
