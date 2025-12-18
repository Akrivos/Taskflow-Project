using MediatR;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries;
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectResponseDto>
{
    private readonly IProjectReadRepository _projectReadRepo;
    public GetProjectQueryHandler(IProjectReadRepository projectReadRepo)
    {
        _projectReadRepo = projectReadRepo;
    } 

    public async Task<ProjectResponseDto> Handle(GetProjectQuery request, CancellationToken ct)
    {
        var project = await _projectReadRepo.GetByIdAsync(request.id, ct);
        if(project is null)
        {
            throw new NotFoundException("Project", request.id);
        }
        return new ProjectResponseDto(project.Id, project.Name, project.Description);
    }
}
