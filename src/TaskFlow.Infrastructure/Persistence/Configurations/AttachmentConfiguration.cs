using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public class AttachmenttConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(a => a.BlobUrl)
            .IsRequired();

        builder.Property(a => a.ContentType)
            .IsRequired();

        builder.Property(a => a.FileName)
            .IsRequired();

        builder.Property(a => a.TaskItemId)
            .IsRequired();

        builder.HasOne(a => a.TaskItem)
            .WithMany()
            .HasForeignKey(a => a.TaskItemId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
