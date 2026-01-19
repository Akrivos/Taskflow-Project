using MediatR;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public sealed class GetLatestsByTaskIdQueryHandler
    : IRequestHandler<GetLatestsByTaskIdQuery, IReadOnlyList<LatestCommentItem>>
{
    private readonly ICommentReadRepository _commentReadRepo;

    public GetLatestsByTaskIdQueryHandler(ICommentReadRepository commentReadRepo)
    {
        _commentReadRepo = commentReadRepo;
    }

    public Task<IReadOnlyList<LatestCommentItem>> Handle(
        GetLatestsByTaskIdQuery request,
        CancellationToken ct)
    {
        var limit = request.Limit ?? 10;
        var sortDirection = request.SortDirection ?? SortDirection.Desc;
        var sortBy = request.SortBy ?? CommentSortBy.CreatedAt;

        return _commentReadRepo.GetLatestsByTaskIdAsync(
            request.TaskId,
            limit,
            sortDirection,
            sortBy,
            ct
        );
    }
}
