namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
public sealed record LatestCommentItem(
   Guid Id,
   string Content,
   DateTimeOffset CreatedAt,
   string UserId,
   TaskSummary Task
);
