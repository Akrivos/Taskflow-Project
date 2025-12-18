using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries;

public class GetProjectsQueryHandler
    : IRequestHandler<GetProjectsQuery, PagedResult<ProjectResponseDto>>
{
    private readonly IProjectReadRepository _projectReadRepo;

    public GetProjectsQueryHandler(IProjectReadRepository projectReadRepo)
    {
           _projectReadRepo = projectReadRepo;
    }

    public async Task<PagedResult<ProjectResponseDto>> Handle(
        GetProjectsQuery request,
        CancellationToken ct)
    {
        var projects = await _projectReadRepo.GetProjectsAsync(
                request.PageNumber,
                request.PageSize,
                request.Search,
                request.SortBy,
                request.SortDirection,
                ct
            );

        var mappedItems = projects.Items
               .Select(p => new ProjectResponseDto(
                    p.Id,
                    p.Name,
                    p.Description
                 ))
                .ToList();

        return new PagedResult<ProjectResponseDto>(
            mappedItems,
            projects.PageNumber,
            projects.PageSize,
            projects.TotalCount
        );
    }
}
