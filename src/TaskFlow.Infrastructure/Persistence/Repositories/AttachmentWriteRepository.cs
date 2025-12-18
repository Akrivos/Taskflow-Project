using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class AttachmentWriteRepository : IAttachmentWriteRepository
{
    private readonly TaskFlowDbContext _db;

    public AttachmentWriteRepository(TaskFlowDbContext db) => _db = db;
    public async Task AddAsync(Attachment entity, CancellationToken ct)
    {
        await _db.Attachments.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}
