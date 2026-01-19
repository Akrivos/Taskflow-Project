using MediatR;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery(
    int? PageNumber,
    int? PageSize,
    string? Search,
    ProjectSortBy? SortBy,
    SortDirection? SortDirection
) : IRequest<PagedResult<ProjectResponseDto>>;
