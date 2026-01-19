using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var refreshToken = await _authService.RefreshTokenAsync(request.RefreshToken, ct);
        if (refreshToken is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        return new AuthResponseDto
        {
            AccessToken = refreshToken.AccessToken,
            RefreshToken = refreshToken.RefreshToken,
            Roles = refreshToken.Roles.ToList()
        };
    }
}