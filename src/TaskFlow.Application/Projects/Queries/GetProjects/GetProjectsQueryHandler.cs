using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Projects.Queries.GetProjects;

namespace TaskFlow.Application.Projects.Queries;

public class GetProjectsQueryHandler
    : IRequestHandler<GetProjectsQuery, PagedResult<ProjectResponseDto>>
{
    private readonly IProjectReadRepository _projectReadRepo;

    public GetProjectsQueryHandler(IProjectReadRepository projectReadRepo)
    {
           _projectReadRepo = projectReadRepo;
    }

    public async Task<PagedResult<ProjectResponseDto>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var pageNumber = request.PageNumber is > 0 ? request.PageNumber.Value : 1;
        var pageSize = request.PageSize is > 0 and <= 100 ? request.PageSize.Value : 20;

        var sortBy = request.SortBy ?? ProjectSortBy.Name;
        var sortDirection = request.SortDirection ?? SortDirection.Asc;

        var projects = await _projectReadRepo.GetProjectsAsync(
            pageNumber,
            pageSize,
            request.Search,
            sortBy,
            sortDirection,
            ct);

        var mappedItems = projects.Items
            .Select(p => new ProjectResponseDto(p.Id, p.Name, p.Description))
            .ToList();

        return new PagedResult<ProjectResponseDto>(
            mappedItems,
            projects.PageNumber,
            projects.PageSize,
            projects.TotalCount
        );
    }
}
