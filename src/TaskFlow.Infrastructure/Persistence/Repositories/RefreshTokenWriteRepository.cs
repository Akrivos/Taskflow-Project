using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class RefreshTokenWriteRepository : IRefreshTokenWriteRepository
{
    private readonly TaskFlowDbContext _db;

    public RefreshTokenWriteRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken> CreateAsync(RefreshTokenBodyDto refreshToken, CancellationToken ct = default)
    {
        var token = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpiresAt = refreshToken.ExpiresAt,
            RevokedAt = refreshToken.RevokedAt,
            ReplacedByToken = refreshToken.ReplacedByToken
        };

        await _db.RefreshTokens.AddAsync(token, ct);
        return token;
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var t in tokens)
        {
            t.RevokedAt = DateTime.UtcNow;
        }
    }

    public async Task RotateAsync(string oldToken, string newToken, CancellationToken ct = default)
    {
        await _db.RefreshTokens
            .Where(rt => rt.Token == oldToken)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(rt => rt.RevokedAt, _ => DateTime.UtcNow)
                    .SetProperty(rt => rt.ReplacedByToken, _ => newToken),
                ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
