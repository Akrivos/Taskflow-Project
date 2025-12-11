using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IRefreshTokenWriteRepository
{
    Task<RefreshToken?> CreateAsync(RefreshTokenBodyDto refreshToken, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
