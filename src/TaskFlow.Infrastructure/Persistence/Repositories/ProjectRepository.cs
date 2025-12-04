using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _db;

    public ProjectRepository(TaskFlowDbContext db) => _db = db;

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Projects.FindAsync(new object?[] { id }, ct);

    public async Task AddAsync(Project entity, CancellationToken ct)
    {
        await _db.Projects.AddAsync(entity, ct);
    }

    public async Task<IEnumerable<Project>> FindAsync(Expression<Func<Project, bool>> predicate, CancellationToken ct) =>
        await _db.Projects.AsNoTracking().Where(predicate).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
