using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class CommentWriteRepository : ICommentWriteRepository
{
    private readonly TaskFlowDbContext _db;

    public CommentWriteRepository(TaskFlowDbContext db)
    {
        _db = db;
    } 

    public async Task AddAsync(Comment entity, CancellationToken ct)
    {
        await _db.Comment.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }    

    public Task DeleteAsync(Comment entity, CancellationToken ct)
    {
        _db.Comment.Remove(entity);
        return Task.CompletedTask;
    }
}
