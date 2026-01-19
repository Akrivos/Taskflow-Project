using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
public class GetProjectsWIthMembersQueryHandler : IRequestHandler<GetProjectsWithMembersQuery, PagedResult<GetProjectsWithMembersResponseDto>>
{
    private readonly IProjectReadRepository _projectReadRepo;
    public GetProjectsWIthMembersQueryHandler(IProjectReadRepository projectReadRepo)
    {
        _projectReadRepo = projectReadRepo;
    }
    public async Task<PagedResult<GetProjectsWithMembersResponseDto>> Handle(
        GetProjectsWithMembersQuery request,
        CancellationToken ct)
    {
        var pageNumber = request.PageNumber is > 0 ? request.PageNumber.Value : 1;
        var pageSize = request.PageSize is > 0 and <= 100 ? request.PageSize.Value : 20;

        var sortBy = request.SortBy ?? ProjectWithMembersSortBy.CreatedAt;
        var sortDirection = request.SortDirection ?? SortDirection.Asc;

        var projectsWithMembers = await _projectReadRepo.GetProjectsWithMembersAsync(
            pageNumber,
            pageSize,
            request.Search,
            sortBy,
            sortDirection,
            ct);

        var mappedItems = projectsWithMembers.Items.Select(p => new GetProjectsWithMembersResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.CreatedAt,
            p.Tasks.Select(t => new ProjectTaskDetails(
                t.Id, t.Title, t.Description, t.Status
            )).ToList(),
            p.Members.Select(m => new ProjectMembersDetails(
                m.Id, m.UserId, m.Role
            )).ToList()
        )).ToList();

        return new PagedResult<GetProjectsWithMembersResponseDto>(
            mappedItems,
            projectsWithMembers.PageNumber,
            projectsWithMembers.PageSize,
            projectsWithMembers.TotalCount
        );
    }
}
