using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentReadRepository
{
    Task<Comment> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LatestCommentItem>> GetLatestsByTaskIdAsync(
        Guid taskId,
        int limit,
        SortDirection sortDirection,
        CommentSortBy sortBy,
        CancellationToken ct = default);
}