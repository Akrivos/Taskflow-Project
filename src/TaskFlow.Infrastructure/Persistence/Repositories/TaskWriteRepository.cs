using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class TaskWriteRepository : ITaskWriteRepository
{
    private readonly TaskFlowDbContext _db;

    public TaskWriteRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TaskItem entity, CancellationToken ct)
    {
        await _db.Tasks.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
       return _db.SaveChangesAsync(ct);
    }
}
