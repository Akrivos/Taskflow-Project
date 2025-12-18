using MediatR;
using TaskFlow.Application.Common.Interfaces;
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
        CancellationToken ct
       )
    {
        var projectsWithMembers = await _projectReadRepo.GetProjectsWithMembersAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.SortDirection,
            ct);

        var mappedItems = projectsWithMembers.Items.Select(p => new GetProjectsWithMembersResponseDto(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.Tasks.Select(t => new ProjectTaskDetails(t.Id, t.Title, t.Description, t.Status)).ToList(),
                p.Members.Select(m => new ProjectMembersDetails(m.Id, m.UserId, m.Role)).ToList()
            )).ToList();

        return new PagedResult<GetProjectsWithMembersResponseDto>(
            projectsWithMembers.Items.Select(p => new GetProjectsWithMembersResponseDto(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.Tasks.Select(t => new ProjectTaskDetails(t.Id, t.Title, t.Description, t.Status)).ToList(),
                p.Members.Select(m => new ProjectMembersDetails(m.Id, m.UserId, m.Role)).ToList()
            )).ToList(),
            projectsWithMembers.PageNumber,
            projectsWithMembers.PageSize,
            projectsWithMembers.TotalCount
        );
    }
}
