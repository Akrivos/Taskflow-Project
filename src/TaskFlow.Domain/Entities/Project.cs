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
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PartialUpdate(string? name, string? description)
    {
        if (name != null && !string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (description != null)
        {
            Description = description == "" ? null : description;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
