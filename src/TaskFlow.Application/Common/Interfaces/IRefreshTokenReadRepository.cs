using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface IRefreshTokenReadRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
}