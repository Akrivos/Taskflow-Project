using MediatR;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Application.Common.Interfaces;

public class GetLatestsByTaskIdQueryHandler : IRequestHandler<GetLatestsByTaskIdQuery, IEnumerable<LatestCommentItem>>
{
    protected readonly ICommentReadRepository _commentReadRepo;
    public GetLatestsByTaskIdQueryHandler(ICommentReadRepository commentReadRepo)
    {
        _commentReadRepo = commentReadRepo;
    }
    public async Task<IEnumerable<LatestCommentItem>> Handle(
        GetLatestsByTaskIdQuery request,
        CancellationToken ct
        )
    {
        var comments = await _commentReadRepo.GetLatestsByTaskIdAsync(
            request.TaskId,
            request.Limit ?? 10,
            request.SortDirection ?? "desc",
            request.SortBy ?? "createdAt",
            ct
        );
        return comments;
    }
}