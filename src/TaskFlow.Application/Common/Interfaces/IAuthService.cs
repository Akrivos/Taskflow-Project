using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}