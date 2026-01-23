using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Data.SqlClient;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.IntegrationTests.Infrastructure;
using Testcontainers.MsSql;
using Xunit;

public class TaskFlowApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

    private string _connectionString = default!;
    private Respawner _respawner = default!;
    private DbConnection _respawnConnection = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IQueueService));
            services.AddSingleton<IQueueService, FakeQueueService>();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                o.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName,
                _ => { });

            services.RemoveAll(typeof(DbContextOptions<TaskFlowDbContext>));
            services.AddDbContext<TaskFlowDbContext>(opt =>
            {
                opt.UseSqlServer(_connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connectionString = _dbContainer.GetConnectionString();

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
            await db.Database.MigrateAsync();
        }

        _respawnConnection = new SqlConnection(_connectionString);
        await _respawnConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_respawnConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = new Table[]
            {
                new Table("dbo", "__EFMigrationsHistory")
            }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_respawnConnection);
    }

    public async Task DisposeAsync()
    {
        if (_respawnConnection is not null)
            await _respawnConnection.DisposeAsync();

        await _dbContainer.DisposeAsync();
    }
}
