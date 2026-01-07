using MediatR;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Auth.Commands.Login;

public record LoginCommand(string UserName, string Password): IRequest<AuthResponseDto>;

