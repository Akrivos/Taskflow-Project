using MediatR;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
public record GetProjectsWithMembersQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = "name",
    string? SortDirection = "asc"
) : IRequest<PagedResult<GetProjectsWithMembersResponseDto>>;