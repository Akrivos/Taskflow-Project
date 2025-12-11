using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IRefreshTokenWriteRepository
{
    Task<RefreshToken?> CreateAsync(RefreshTokenBodyDto refreshToken, CancellationToken ct = default);
    Task RevokeAllForUserAsync (string userId, CancellationToken ct = default);
    Task RotateAsync(string oldToken, string newToken, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
