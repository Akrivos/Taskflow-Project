using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using TaskFlow.Application.Common.Interfaces;          
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Files;                    
using TaskFlow.Infrastructure.Messaging;                
using TaskFlow.Infrastructure.Persistence;              
using TaskFlow.Infrastructure.Persistence.Repositories;
using TaskFlow.Infrastructure.Persistence.Services;

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
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<ICommentReadRepository, CommentReadRepository>();
        services.AddScoped<IRepository<Project>, ProjectRepository>();
        services.AddScoped<IRepository<TaskItem>, TaskRepository>();
        services.AddScoped<IRefreshTokenReadRepository, RefreshTokenReadRepository>();
        services.AddScoped<IRefreshTokenWriteRepository, RefreshTokenWriteRepository>();

        // ---------------- Services ----------------
        //services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagerService, UserManagerService>();
        services.AddScoped<IRoleManagerService, RoleManagerService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // ---------------- Azure Blob ----------------
        services.Configure<BlobRetryOptions>(config.GetSection("AzureBlobStorage:Retry"));

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
                HostName = config["RabbitMQ:HostName"],
                UserName = config["RabbitMQ:UserName"],
                Password = config["RabbitMQ:Password"]
            });

        services.AddSingleton<IQueueService, RabbitMqPublisher>();

        return services;
    }
}
