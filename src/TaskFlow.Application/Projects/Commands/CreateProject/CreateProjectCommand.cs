using MediatR;
namespace TaskFlow.Application.Projects.Commands;
public sealed record CreateProjectCommand(string Name, string? Description) : IRequest<Guid>;
