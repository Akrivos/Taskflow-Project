using MediatR;
using TaskFlow.Application.DTOs;
namespace TaskFlow.Application.Auth.Commands.RefreshTokenCommand;
public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;