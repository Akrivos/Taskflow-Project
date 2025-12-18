using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence;
public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Comment> Comment => Set<Comment>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);
    }
}
