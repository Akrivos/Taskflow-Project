using MediatR;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
public sealed record GetProjectsWithMembersQuery(
    int? PageNumber,
    int? PageSize,
    string? Search,
    ProjectWithMembersSortBy? SortBy,
    SortDirection? SortDirection
) : IRequest<PagedResult<GetProjectsWithMembersResponseDto>>;