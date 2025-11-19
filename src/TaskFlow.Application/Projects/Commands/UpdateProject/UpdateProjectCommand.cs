using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public record UpdateProjectCommand(Guid Id, string? Name, string? Description) : IRequest<Guid>;

