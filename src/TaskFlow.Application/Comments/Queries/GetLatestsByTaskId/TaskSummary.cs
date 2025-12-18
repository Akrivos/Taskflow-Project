namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
public sealed record TaskSummary(
    Guid Id,
    string Title,
    string? Description
);