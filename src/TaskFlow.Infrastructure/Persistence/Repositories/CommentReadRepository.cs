using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;
public sealed class CommentReadRepository : ICommentReadRepository
{
    private readonly TaskFlowDbContext _db;
    public CommentReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Comment.SingleOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<IReadOnlyList<LatestCommentItem>> GetLatestsByTaskIdAsync(
        Guid taskId,
        int limit,
        SortDirection sortDirection,
        CommentSortBy sortBy,
        CancellationToken ct = default)
    {
        var query = _db.Comment
            .AsNoTracking()
            .Where(c => c.TaskItemId == taskId);

        query = (sortBy, sortDirection) switch
        {
            (CommentSortBy.CreatedAt, SortDirection.Asc) => query.OrderBy(c => c.CreatedAt),
            (CommentSortBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        return await query
            .Take(limit)
            .Select(c => new LatestCommentItem(
                c.Id,
                c.Content,
                c.CreatedAt,
                c.UserId,
                new TaskSummary(
                    c.TaskItemId,
                    c.TaskItem.Title,
                    c.TaskItem.Description
                )
            ))
            .ToListAsync(ct);
    }
}
