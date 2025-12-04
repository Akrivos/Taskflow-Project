using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class ProjectMember
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    
    public Guid ProjectId { get; set; } = default!;
    public Project Project { get; set; } = default!;
    public ProjectMemberRoles Role { get; set; } = ProjectMemberRoles.Viewer;

    public void SetRole(ProjectMemberRoles role)
    {
        Role = role;
    }

}
