using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentWriteRepository
{
    Task AddAsync(Comment entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct) ;
    Task DeleteAsync(Comment entity, CancellationToken ct);
}