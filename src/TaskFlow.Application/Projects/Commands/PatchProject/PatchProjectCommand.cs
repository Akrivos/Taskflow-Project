using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public record PatchProjectCommand(Guid Id, string? Name, string? Description) : IRequest<Guid>;
