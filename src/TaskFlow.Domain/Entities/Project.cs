using TaskFlow.Domain.Common;
namespace TaskFlow.Domain.Entities;
public class Project : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();
    private Project() { }
    public Project(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
