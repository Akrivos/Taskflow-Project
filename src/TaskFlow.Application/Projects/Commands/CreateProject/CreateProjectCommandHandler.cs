using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Projects.Commands;
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository _repo;
    private readonly IQueueService _queue;
    private readonly ICurrentUser _currentUser;
    public CreateProjectCommandHandler(ICurrentUser currentUser, IProjectRepository repo, IQueueService queue)
    {
        _currentUser = currentUser;
        _repo = repo; 
        _queue = queue;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        Console.WriteLine($"UserId: {_currentUser.UserId}, HasAllowedRole: {hasAllowedRole}");
        if (_currentUser.UserId == null || hasAllowedRole)
        {
            throw new ForbiddenAccessException("You dont have access!");
        }
  
        var entity = new Project(request.Name, request.Description);
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        await _queue.PublishAsync("project-created", System.Text.Json.JsonSerializer.Serialize(new { entity.Id, entity.Name }), ct);
        return entity.Id;
    }
}
