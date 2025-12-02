using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Persistence.Repositories;
public sealed class CommentReadRepository : ICommentReadRepository
{
    private readonly TaskFlowDbContext _db;
    public CommentReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }
    public async Task<IEnumerable<GetLatestCommentsResponseDto>> GetLatestsByTaskIdAsync(
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

        return await query.Take(limit).Select(c => new GetLatestCommentsResponseDto(
            c.Id,
            c.Content,
            c.CreatedAt,
            c.UserId,
            new TaskDetails(
                c.TaskItem.Id,
                c.TaskItem.Title,
                c.TaskItem.Description
            )
        )).ToListAsync(ct);
    }
}
