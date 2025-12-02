namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentReadRepository
{
    Task<IEnumerable<GetLatestCommentsResponseDto>> GetLatestsByTaskIdAsync(Guid taskId, int limit, string sortDirection, string sortBy, CancellationToken ct);
}