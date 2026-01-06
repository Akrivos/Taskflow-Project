using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;
public interface ITaskWriteRepository
{
    Task AddAsync(TaskItem task, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
