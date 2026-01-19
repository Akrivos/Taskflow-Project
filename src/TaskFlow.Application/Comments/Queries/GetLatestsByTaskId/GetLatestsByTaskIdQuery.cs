using MediatR;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public sealed record GetLatestsByTaskIdQuery(
    Guid TaskId,
    int? Limit,
    SortDirection? SortDirection,
    CommentSortBy? SortBy
) : IRequest<IReadOnlyList<LatestCommentItem>>;
