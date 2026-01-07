using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public class RefreshTokenReadRepository : IRefreshTokenReadRepository
{
    private readonly TaskFlowDbContext _db;
    public RefreshTokenReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await _db.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Token == token, ct);
    }
}