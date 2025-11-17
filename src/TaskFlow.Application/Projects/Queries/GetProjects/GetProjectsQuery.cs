using MediatR;
using TaskFlow.Application.DTOs;

public record GetProjectsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = "name",
    string? SortDirection = "asc"
) : IRequest<PagedResult<ProjectDto>>;
