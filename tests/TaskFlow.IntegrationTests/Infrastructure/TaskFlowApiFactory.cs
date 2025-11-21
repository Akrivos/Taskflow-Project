using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;

public class TaskFlowApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TaskFlowTestDb_{Guid.NewGuid()}";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var tempConfig = new ConfigurationBuilder()
                .AddConfiguration(configBuilder.Build())
                .Build();

            var cs = tempConfig.GetConnectionString("DefaultConnection");

            cs = cs.Replace("TaskFlowTestDb_Template", _dbName);

            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = cs
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IQueueService));
            services.AddSingleton<IQueueService, FakeQueueService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName,
                _ => { }
             );
        });

        var host = base.CreateHost(builder);

        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
            db.Database.EnsureCreated();
        }

        return host;
    }
}
