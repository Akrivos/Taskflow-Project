using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries;

public class GetProjectsQueryHandler
    : IRequestHandler<GetProjectsQuery, PagedResult<ProjectDto>>
{
    private readonly IProjectReadRepository _projects;

    public GetProjectsQueryHandler(IProjectReadRepository projects)
        => _projects = projects;

    public Task<PagedResult<ProjectDto>> Handle(
        GetProjectsQuery request,
        CancellationToken ct)
    {
        return _projects.GetProjectsAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.SortDirection,
            ct);
    }
}
