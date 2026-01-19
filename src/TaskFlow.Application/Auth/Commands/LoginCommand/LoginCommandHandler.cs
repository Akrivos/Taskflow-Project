using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Auth.Commands.Login;


public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    public LoginCommandHandler(
        IAuthService authService
      )
    {
        _authService = authService;
    }
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var authResponse = await _authService.LoginAsync(request.UserName, request.Password, ct);
        if(authResponse is null)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        return new AuthResponseDto
        {
            AccessToken = authResponse.AccessToken,
            RefreshToken = authResponse.RefreshToken,
            Roles = authResponse.Roles.ToList()
        };
    }
}

