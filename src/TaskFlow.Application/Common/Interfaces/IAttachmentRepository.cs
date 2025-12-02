
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        Task DeleteAsync(Attachment entity, CancellationToken ct);
    }
}
