namespace TaskFlow.Application.DTOs;

using TaskFlow.Domain.Enums;
using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

public record ProjectTaskDetails(Guid Id, string Title, string? Description, DomainTaskStatus Status);
public record ProjectMembersDetails(Guid Id, string UserId, ProjectMemberRoles Role);
public record GetProjectsWithMembersResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    List<ProjectTaskDetails> Tasks,
    List<ProjectMembersDetails> Members
 );
