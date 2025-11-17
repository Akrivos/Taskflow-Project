using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IProjectReadRepository 
    {
        Task<PagedResult<ProjectDto>> GetProjectsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDirection,
            CancellationToken ct);
    }
}
