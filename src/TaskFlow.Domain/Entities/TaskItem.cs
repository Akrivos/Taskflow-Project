using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;
using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Domain.Entities;
public class TaskItem : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DomainTaskStatus Status { get; private set; } = DomainTaskStatus.New;
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = default!;
    public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
    private TaskItem() {}
    public TaskItem(string title, string? description, Guid projectId)
    {
        Title = title;
        Description = description;
        ProjectId = projectId;
    }
}
