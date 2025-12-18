using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;
public sealed class CommentReadRepository : ICommentReadRepository
{
    private readonly TaskFlowDbContext _db;
    public CommentReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Comment.FindAsync(new { id }, ct);
    }
    public async Task<IEnumerable<LatestCommentItem>> GetLatestsByTaskIdAsync(
        Guid taskId,
        int limit,
        string sortDirection,
        string sortBy,
        CancellationToken ct)
    {
        var query = _db.Comment.AsNoTracking().Where(c => c.TaskItemId == taskId);

        query = (sortBy.ToLower(), sortDirection.ToLower()) switch
        {
            ("CreatedAt", "asc") => query.OrderBy(c => c.CreatedAt),
            ("CreatedAt", "desc") => query.OrderByDescending(c => c.CreatedAt),
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
                    c.TaskItem.Id,
                    c.TaskItem.Title,
                    c.TaskItem.Description
                )
            )).ToListAsync(ct);
    }
}
