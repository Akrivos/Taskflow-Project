using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries;
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IProjectRepository _repo;
    public GetProjectQueryHandler(IProjectRepository repo) => _repo = repo;

    public async Task<ProjectDto> Handle(GetProjectQuery request, CancellationToken ct)
    {
        var project = await _repo.GetByIdAsync(request.id, ct);
        if(project == null)
        {
            throw new NotFoundException("Project", request.id);
        }
        return new ProjectDto(project.Id, project.Name, project.Description);
    }
}
