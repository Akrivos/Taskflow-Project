using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Azure.Storage.Blobs;

using RabbitMQ.Client;

using TaskFlow.Application.Common.Interfaces;           // IBlobService, IQueueService, repos interfaces
using TaskFlow.Infrastructure.Files;                    // AzureBlobService
using TaskFlow.Infrastructure.Messaging;                // RabbitMqPublisher
using TaskFlow.Infrastructure.Persistence;              // TaskFlowDbContext
using TaskFlow.Infrastructure.Persistence.Repositories; // ProjectRepository, TaskRepository

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ---------------- EF Core ----------------
        services.AddDbContext<TaskFlowDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // ---------------- Repositories ----------------
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        // Αν χρησιμοποιείς και generic repo παράλληλα:
        services.AddScoped<IRepository<TaskFlow.Domain.Entities.Project>, ProjectRepository>();
        services.AddScoped<IRepository<TaskFlow.Domain.Entities.TaskItem>, TaskRepository>();

        // ---------------- Azure Blob ----------------
        services.AddSingleton(sp =>
        {
            var cs = config["Azure:BlobConnectionString"];
            return new BlobServiceClient(cs);
        });
        services.AddScoped<IBlobService, AzureBlobService>();

        // ---------------- RabbitMQ ----------------
        services.AddSingleton<IConnectionFactory>(sp =>
            new ConnectionFactory
            {
                HostName = config["RabbitMQ:HostName"] ?? "localhost",
                UserName = config["RabbitMQ:UserName"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest"
            });

        services.AddSingleton<IQueueService, RabbitMqPublisher>();

        return services;
    }
}
