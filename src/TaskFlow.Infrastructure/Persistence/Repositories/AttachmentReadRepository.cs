using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class AttachmentReadRepository : IAttachmentReadRepository
{
    private readonly TaskFlowDbContext _db;

    public AttachmentReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Attachment?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Attachments.FindAsync(new { id }, ct);
    }
}
