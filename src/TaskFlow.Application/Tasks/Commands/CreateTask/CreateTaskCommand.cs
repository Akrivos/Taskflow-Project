using MediatR;
namespace TaskFlow.Application.Tasks.Commands;
public record CreateTaskCommand(string Title, string? Description, Guid ProjectId) : IRequest<Guid>;
