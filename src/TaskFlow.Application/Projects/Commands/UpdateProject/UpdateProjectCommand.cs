using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public sealed record UpdateProjectCommand(Guid Id, string? Name, string? Description) : IRequest<Guid>;

