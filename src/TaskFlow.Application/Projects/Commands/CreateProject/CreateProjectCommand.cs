using MediatR;
namespace TaskFlow.Application.Projects.Commands;
public record CreateProjectCommand(string Name, string? Description) : IRequest<Guid>;
