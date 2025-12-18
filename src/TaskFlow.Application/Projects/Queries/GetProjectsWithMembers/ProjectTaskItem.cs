namespace TaskFlow.Application.Projects.Queries.GetProjectsWithMembers;

using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

public sealed record ProjectTaskItem(Guid Id, string Title, string? Description, DomainTaskStatus Status);
