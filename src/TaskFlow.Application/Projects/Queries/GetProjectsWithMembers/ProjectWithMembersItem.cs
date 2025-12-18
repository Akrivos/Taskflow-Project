namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;

public sealed record ProjectWithMembersItem(
    Guid Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    List<ProjectTaskItem> Tasks,
    List<ProjectMemberItem> Members
);