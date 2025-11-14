using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Common.Behaviors;

namespace TaskFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR (v12/13 style)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // 🔹 Αυτό σκανάρει ΟΛΑ τα classes που κληρονομούν από AbstractValidator<T>
        services.AddValidatorsFromAssembly(assembly);

        // 🔹 Εγγραφή του validation behavior στο MediatR pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
