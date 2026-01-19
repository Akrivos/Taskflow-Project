using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Projects.Queries.GetProjects;
using TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IProjectReadRepository 
    {
        Task<Project> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<ProjectListItem>> GetProjectsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            ProjectSortBy? sortBy,
            SortDirection? sortDirection,
            CancellationToken ct = default);

        Task<PagedResult<ProjectWithMembersItem>> GetProjectsWithMembersAsync(
            int pageNumber,
            int pageSize,
            string? search,
            ProjectWithMembersSortBy? sortBy,
            SortDirection? sortDirection,
            CancellationToken ct = default);
    }
}
