namespace TaskFlow.Api.Controllers.Requests.Comment;
public sealed record CreateCommentRequest(
    Guid TaskId,
    string Content
);