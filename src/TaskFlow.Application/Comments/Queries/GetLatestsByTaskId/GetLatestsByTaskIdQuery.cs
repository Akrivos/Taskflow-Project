using MediatR;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public sealed record GetLatestsByTaskIdQuery(
    Guid TaskId,
    int? Limit = 10,
    string? SortDirection = "desc",
    string? SortBy = "createdAt"
) : IRequest<IEnumerable<LatestCommentItem>>;