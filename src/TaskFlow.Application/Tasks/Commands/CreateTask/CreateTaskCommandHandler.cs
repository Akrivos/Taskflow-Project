using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Tasks.Commands;
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskRepository _repo;
    private readonly IQueueService _queue;
    public CreateTaskCommandHandler(ITaskRepository repo, IQueueService queue) { _repo = repo; _queue = queue; }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var entity = new TaskItem(request.Title, request.Description, request.ProjectId);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        await _queue.PublishAsync("task-created", System.Text.Json.JsonSerializer.Serialize(new { entity.Id, entity.Title }), ct);
        return entity.Id;
    }
}
