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

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
