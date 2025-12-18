using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class TaskReadRepository : ITaskReadRepository
{
    private readonly TaskFlowDbContext _db;
    public TaskReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Tasks.FindAsync(new { id }, ct);
    }
}
