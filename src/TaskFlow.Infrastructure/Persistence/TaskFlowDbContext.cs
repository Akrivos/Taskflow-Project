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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Project>().Property(p => p.Name).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<TaskItem>().Property(t => t.Title).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Attachment>().Property(a => a.FileName).IsRequired();

        // Αυτό φορτώνει ΟΛΕΣ τις *IEntityTypeConfiguration<T>* που βρίσκονται στο assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);
    }
}
