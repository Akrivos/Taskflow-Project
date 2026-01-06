using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;
public interface IProjectWriteRepository 
{
    Task AddAsync(Project project, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
