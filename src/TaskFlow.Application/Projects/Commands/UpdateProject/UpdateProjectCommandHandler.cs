using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Projects.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Guid>
{
    private readonly IProjectRepository _repo;
    private readonly IQueueService _queue;
    private readonly ICurrentUser _currentUser;
    public UpdateProjectCommandHandler(ICurrentUser currentUser, IProjectRepository repo, IQueueService queue)
    {
        _currentUser = currentUser;
        _repo = repo;
        _queue = queue;
    }

    public async Task<Guid> Handle(UpdateProjectCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        if (_currentUser.UserId == null || hasAllowedRole)
        {
            throw new ForbiddenAccessException("You dont have access!");
        }

        var project = await _repo.GetByIdAsync(request.Id, ct);
        if(project == null)
        {
            throw new NotFoundException("Project", request.Id);
        }

        project.Update(request.Name, request.Description);
        
        await _repo.SaveChangesAsync(ct);
        await _queue.PublishAsync("project-updated", System.Text.Json.JsonSerializer.Serialize(new { project.Id, project.Name, project.Description }), ct);
        return project.Id;
    }
}
