namespace TaskFlow.Application.DTOs;

public record CommentDto(
    Guid Id,
    Guid TaskItemId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);