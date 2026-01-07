using MediatR;
using System.Text.Json;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Projects.Commands;
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectWriteRepository _projectWriteRepo;
    private readonly IQueueService _queue;
    private readonly ICurrentUser _currentUser;
    public CreateProjectCommandHandler(ICurrentUser currentUser, IProjectWriteRepository projectWriteRepo, IQueueService queue)
    {
        _currentUser = currentUser;
        _projectWriteRepo = projectWriteRepo; 
        _queue = queue;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var hasAllowedRole = !_currentUser.IsInRole("ProjectManager") && !_currentUser.IsInRole("Admin");
        if (_currentUser.UserId is null || hasAllowedRole)
        {
            throw new ForbiddenException("You dont have access!");
        }
      
        var project = new Project(request.Name, request.Description);
        await _projectWriteRepo.AddAsync(project, ct);
        await _projectWriteRepo.SaveChangesAsync(ct);
        await _queue.PublishAsync("project-created", JsonSerializer.Serialize(new { project.Id, project.Name }), ct);
        return project.Id;
    }
}
