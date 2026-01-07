using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentWriteRepository
{
    Task AddAsync(Comment entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default) ;
    Task DeleteAsync(Comment entity, CancellationToken ct = default);
}