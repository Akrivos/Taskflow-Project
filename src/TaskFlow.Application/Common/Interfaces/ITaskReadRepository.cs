using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ITaskReadRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
}