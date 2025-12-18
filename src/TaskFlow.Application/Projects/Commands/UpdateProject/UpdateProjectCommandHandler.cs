using MediatR;
using System.Text.Json;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Application.Projects.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Guid>
{
    private readonly IProjectWriteRepository _projectWriteRepo;
    private readonly IProjectReadRepository _projectReadRepo;
    private readonly IQueueService _queue;
    private readonly ICurrentUser _currentUser;
    public UpdateProjectCommandHandler(
        ICurrentUser currentUser, 
        IProjectWriteRepository projectWriteRepo,
        IProjectReadRepository projectReadRepo,
        IQueueService queue
       )
    {
        _currentUser = currentUser;
        _projectWriteRepo = projectWriteRepo;
        _projectReadRepo = projectReadRepo;
        _queue = queue;
    }

    public async Task<Guid> Handle(UpdateProjectCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        if (_currentUser.UserId is null || hasAllowedRole)
        {
            throw new ForbiddenAccessException("You dont have access!");
        }

        var project = await _projectReadRepo.GetByIdAsync(request.Id, ct);
        if(project is null)
        {
            throw new NotFoundException("Project", request.Id);
        }

        project.Update(request.Name, request.Description);
        
        await _projectWriteRepo.SaveChangesAsync(ct);
        await _queue.PublishAsync("project-updated", JsonSerializer.Serialize(new { project.Id, project.Name, project.Description }), ct);
        return project.Id;
    }
}
