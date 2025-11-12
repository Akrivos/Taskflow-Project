using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries;
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _repo;
    public GetProjectsQueryHandler(IProjectRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var items = await _repo.FindAsync(_ => true, ct);
        return items.Select(p => new ProjectDto(p.Id, p.Name, p.Description));
    }
}
