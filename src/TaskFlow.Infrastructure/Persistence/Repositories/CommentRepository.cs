using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository : ICommentRepository
{
    private readonly TaskFlowDbContext _db;

    public CommentRepository(TaskFlowDbContext db) => _db = db;

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Comment.FindAsync(new object?[] { id }, ct);

    public async Task AddAsync(Comment entity, CancellationToken ct)
    {
        await _db.Comment.AddAsync(entity, ct);
    }

    public async Task<IEnumerable<Comment>> FindAsync(Expression<Func<Comment, bool>> predicate, CancellationToken ct) =>
        await _db.Comment.AsNoTracking().Where(predicate).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public Task DeleteAsync(Comment entity, CancellationToken ct)
    {
        _db.Comment.Remove(entity);
        return Task.CompletedTask;
    }
}
