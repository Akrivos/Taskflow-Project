using MediatR;

namespace TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;

public record GetLatestsByTaskIdQuery(
    Guid TaskId,
    int? Limit = 10,
    string? SortDirection = "desc",
    string? SortBy = "createdAt"
) : IRequest<IEnumerable<GetLatestCommentsResponseDto>>;