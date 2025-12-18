
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IAttachmentWriteRepository
    {
        Task AddAsync(Attachment entity, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
