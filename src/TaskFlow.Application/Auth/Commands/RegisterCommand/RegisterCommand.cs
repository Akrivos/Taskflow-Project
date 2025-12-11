using MediatR;

namespace TaskFlow.Application.Auth.Commands.RegisterCommand;

public record RegisterCommand(string Username, string Email, string Password, string Role) : IRequest<string>;