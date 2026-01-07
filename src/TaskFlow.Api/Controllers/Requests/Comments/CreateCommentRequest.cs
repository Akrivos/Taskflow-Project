namespace TaskFlow.Api.Controllers.Requests.Comment;
public sealed record CreateCommentRequest(
    Guid TaskItemId,
    string Content
);