using TaskFlow.Application.DTOs;
using TaskFlow.Application.Projects.Queries.GetProjects;
using TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IProjectReadRepository 
    {
        Task<Project> GetByIdAsync(Guid id, CancellationToken ct);
        Task<PagedResult<ProjectListItem>> GetProjectsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDirection,
            CancellationToken ct);

        Task<PagedResult<ProjectWithMembersItem>> GetProjectsWithMembersAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDirection,
            CancellationToken ct);
    }
}
