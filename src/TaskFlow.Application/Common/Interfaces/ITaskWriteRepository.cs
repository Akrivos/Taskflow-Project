using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;
public interface ITaskWriteRepository
{
    Task AddAsync(TaskItem task, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
