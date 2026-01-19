using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthTokenResult?> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<AuthTokenResult?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}