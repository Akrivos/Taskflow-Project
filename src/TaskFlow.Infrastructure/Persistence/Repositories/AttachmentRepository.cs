using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class AttachmentRepository : IAttachmentRepository
{
    private readonly TaskFlowDbContext _db;

    public AttachmentRepository(TaskFlowDbContext db) => _db = db;

    public async Task<Attachment?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Attachments.FindAsync(new object?[] { id }, ct);

    public async Task AddAsync(Attachment entity, CancellationToken ct)
    {
        await _db.Attachments.AddAsync(entity, ct);
    }

    public async Task<IEnumerable<Attachment>> FindAsync(Expression<Func<Attachment, bool>> predicate, CancellationToken ct) =>
        await _db.Attachments.AsNoTracking().Where(predicate).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public Task DeleteAsync(Attachment entity, CancellationToken ct)
    {
        _db.Attachments.Remove(entity);
        return Task.CompletedTask;
    }
}
