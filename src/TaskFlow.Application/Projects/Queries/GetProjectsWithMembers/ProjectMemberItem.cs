using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;
public sealed record ProjectMemberItem(Guid Id, string UserId, ProjectMemberRoles Role);
