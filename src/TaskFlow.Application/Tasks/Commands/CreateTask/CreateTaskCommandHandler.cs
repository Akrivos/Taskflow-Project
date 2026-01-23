using MediatR;
using System.Text.Json;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Messages;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Tasks.Commands;
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskWriteRepository _taskWriteRepo;
    private readonly IQueueService _queue;
    public CreateTaskCommandHandler(ITaskWriteRepository repo, IQueueService queue) 
    {
        _taskWriteRepo = repo; 
        _queue = queue; 
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var entity = new TaskItem(request.Title, request.Description, request.ProjectId);
        await _taskWriteRepo.AddAsync(entity, ct);
        await _taskWriteRepo.SaveChangesAsync(ct);
        await _queue.PublishAsync(Topics.TaskCreated, JsonSerializer.Serialize(new { entity.Id, entity.Title }), ct);
        return entity.Id;
    }
}
