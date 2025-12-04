using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            HostName = ctx.Configuration["RabbitMQ:HostName"],
            UserName = ctx.Configuration["RabbitMQ:UserName"],
            Password = ctx.Configuration["RabbitMQ:Password"]
        });

        services.AddHostedService<RabbitConsumer>();
    })
    .Build()
    .RunAsync();

public sealed class RabbitConsumer : BackgroundService
{
    private readonly ILogger<RabbitConsumer> _logger;
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitConsumer(ILogger<RabbitConsumer> logger, IConnectionFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retries = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: "taskflow_notifications",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                break;
            }
            catch (Exception ex) when (retries < 15)
            {
                _logger.LogWarning(ex, "RabbitMQ not ready yet. Retrying in 3s... (Attempt {Attempt}/15)", retries + 1);
                retries++;
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }

        if (_connection is null || _channel is null)
        {
            _logger.LogError("Could not connect to RabbitMQ after retries.");
            return;
        }

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Notification received: {Message}", message);
        };

        _channel.BasicConsume(
            queue: "taskflow_notifications",
            autoAck: true,
            consumer: consumer
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        try { _channel?.Close(); } catch { }
        try { _connection?.Close(); } catch { }
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
