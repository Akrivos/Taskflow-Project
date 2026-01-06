using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectWriteRepository : IProjectWriteRepository
{
    private readonly TaskFlowDbContext _db;

    public ProjectWriteRepository(TaskFlowDbContext db)
    {
        _db = db;
    } 

    public async Task AddAsync(Project entity, CancellationToken ct = default)
    {
        await _db.Projects.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
