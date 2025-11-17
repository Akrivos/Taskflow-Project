using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository : IProjectReadRepository
{
    private readonly TaskFlowDbContext _db;

    public ProjectReadRepository(TaskFlowDbContext db) => _db = db;

    public async Task<PagedResult<ProjectDto>> GetProjectsAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        string? sortDirection,
        CancellationToken ct)
    {
        if (pageNumber < 1) pageNumber = 1;

        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        var sortByNorm = string.IsNullOrWhiteSpace(sortBy)
            ? "name"
            : sortBy.Trim().ToLowerInvariant();

        var sortDirNorm = string.IsNullOrWhiteSpace(sortDirection)
            ? "asc"
            : sortDirection.Trim().ToLowerInvariant();

        query = (sortByNorm, sortDirNorm) switch
        {
            ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", _) => query.OrderBy(p => p.CreatedAt),

            ("name", "desc") => query.OrderByDescending(p => p.Name),
            ("name", _) => query.OrderBy(p => p.Name),

            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.Description
            ))
            .ToListAsync(ct);

        return new PagedResult<ProjectDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

}
