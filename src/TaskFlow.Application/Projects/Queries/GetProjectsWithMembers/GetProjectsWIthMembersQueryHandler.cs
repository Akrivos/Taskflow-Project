using MediatR;
using System;
using System.Collections.Generic;
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
    public async Task<PagedResult<GetProjectsWithMembersResponseDto>> Handle(GetProjectsWithMembersQuery request, CancellationToken ct)
    {
        var projects = await _projectReadRepo.GetProjectsWithMembersAsync(
            request.PageNumber, 
            request.PageSize, 
            request.Search, 
            request.SortBy, 
            request.SortDirection, 
            ct
        );

        return projects;
    }
}
