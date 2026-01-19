using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Projects.Queries.GetProjects;
using TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectReadRepository : IProjectReadRepository
{
    private readonly TaskFlowDbContext _db;

    public ProjectReadRepository(TaskFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Project> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Projects.SingleOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<PagedResult<ProjectListItem>> GetProjectsAsync(
        int pageNumber,
        int pageSize,
        string? search,
        ProjectSortBy? sortBy,
        SortDirection? sortDirection,
        CancellationToken ct = default)
    {
        var query = _db.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        query = (sortBy, sortDirection) switch
        {
            (ProjectSortBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(p => p.CreatedAt),
            (ProjectSortBy.CreatedAt, _) => query.OrderBy(p => p.CreatedAt),

            (ProjectSortBy.Name, SortDirection.Desc) => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectListItem(p.Id, p.Name, p.Description))
            .ToListAsync(ct);

        return new PagedResult<ProjectListItem>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedResult<ProjectWithMembersItem>> GetProjectsWithMembersAsync(
        int pageNumber,
        int pageSize,
        string? search,
        ProjectWithMembersSortBy? sortBy,
        SortDirection? sortDirection,
        CancellationToken ct = default)
    {
        var query = _db.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        query = (sortBy, sortDirection) switch
        {
            (ProjectWithMembersSortBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(p => p.CreatedAt),
            (ProjectWithMembersSortBy.CreatedAt, _) => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectWithMembersItem(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.Tasks.Select(t => new ProjectTaskItem(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status
                )).ToList(),
                p.Members.Select(m => new ProjectMemberItem(
                    m.Id,
                    m.UserId,
                    m.Role
                )).ToList()
            ))
            .ToListAsync(ct);

        return new PagedResult<ProjectWithMembersItem>(items, pageNumber, pageSize, totalCount);
    }
}
