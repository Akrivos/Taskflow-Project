using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public sealed record PatchProjectCommand(Guid Id, string? Name, string? Description) : IRequest<Guid>;
