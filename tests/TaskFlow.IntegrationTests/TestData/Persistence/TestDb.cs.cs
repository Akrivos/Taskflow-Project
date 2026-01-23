using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.IntegrationTests.TestData.Persistence;

public sealed class TestDb
{
    private readonly IServiceProvider _services;

    public TestDb(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<T> AddAsync<T>(T entity) where T : class
    {
        await using var scope = _services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

        db.Set<T>().Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity?> SingleOrDefaultAsync<TEntity>(
        Func<TaskFlowDbContext, IQueryable<TEntity>> query)
        where TEntity : class
    {
        await using var scope = _services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

        return await query(db).SingleOrDefaultAsync();
    }

    public async Task<TEntity> SingleAsync<TEntity>(
        Func<TaskFlowDbContext, IQueryable<TEntity>> query,
        string because = "the entity should exist")
        where TEntity : class
    {
        var entity = await SingleOrDefaultAsync(query);
        if (entity is null)
        {
            throw new InvalidOperationException(because);
        }

        return entity;
    }

    public async Task<bool> AnyAsync<TEntity>(
        Func<TaskFlowDbContext, IQueryable<TEntity>> query)
        where TEntity : class
    {
        await using var scope = _services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

        return await query(db).AnyAsync();
    }
}
