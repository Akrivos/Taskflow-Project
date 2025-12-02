public record TaskDetails (
    Guid Id,
    string Title,
    string? Description
);

public record GetLatestCommentsResponseDto(
   Guid Id,
   string Content,
   DateTimeOffset CreatedAt,
   string UserId,
   TaskDetails Task
);