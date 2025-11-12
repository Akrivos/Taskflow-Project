using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Projects.Commands;
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository _repo;
    private readonly IQueueService _queue;
    public CreateProjectCommandHandler(IProjectRepository repo, IQueueService queue)
    {
        _repo = repo; _queue = queue;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var entity = new Project(request.Name, request.Description);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        await _queue.PublishAsync("project-created", System.Text.Json.JsonSerializer.Serialize(new { entity.Id, entity.Name }), ct);
        return entity.Id;
    }
}
