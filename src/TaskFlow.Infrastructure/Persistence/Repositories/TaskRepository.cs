using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _db;

    public TaskRepository(TaskFlowDbContext db) => _db = db;

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Tasks.FindAsync(new object?[] { id }, ct);

    public async Task AddAsync(TaskItem entity, CancellationToken ct)
    {
        await _db.Tasks.AddAsync(entity, ct);
    }

    public async Task<IEnumerable<TaskItem>> FindAsync(Expression<Func<TaskItem, bool>> predicate, CancellationToken ct) =>
        await _db.Tasks.AsNoTracking().Where(predicate).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
