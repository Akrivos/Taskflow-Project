
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IAttachmentReadRepository
    {
        Task<Attachment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
