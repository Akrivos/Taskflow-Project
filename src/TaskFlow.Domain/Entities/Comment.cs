using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities;

public class Comment : AuditableEntity
{
    public const int MaxContentLength = 2000;

    public string Content { get; private set; } = default!;
    public Guid TaskItemId { get; private set; }
    public TaskItem TaskItem { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    private Comment() { }

    public Comment(Guid taskItemId, string content, string userId)
    {
        TaskItemId = taskItemId;
        Content = content;
        UserId = userId;
    }

    public void Validate()
    {
        if(string.IsNullOrWhiteSpace(Content))
        {
            throw new ArgumentException("Content cannot be empty.");
        }
        if(Content.Length > MaxContentLength)
        {
            throw new ArgumentException($"Content cannot exceed {MaxContentLength} characters.");
        }
        if(string.IsNullOrEmpty(UserId))
        {
            throw new ArgumentException("UserId cannot be empty.");
        }
        if(TaskItemId == Guid.Empty)
        {
            throw new ArgumentException("TaskItemId cannot be empty.");
        }
    }
}
