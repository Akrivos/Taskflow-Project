using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentReadRepository
{
    Task<Comment> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<LatestCommentItem>> GetLatestsByTaskIdAsync(Guid taskId, int limit, string sortDirection, string sortBy, CancellationToken ct);
}