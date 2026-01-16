using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskFlow.Application.Common.Behaviours;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Files;

namespace TaskFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        services.AddScoped<IFileUploadService, FileUploadService>();

        return services;
    }
}
