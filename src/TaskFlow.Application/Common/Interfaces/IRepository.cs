using System.Linq.Expressions;

namespace TaskFlow.Application.Common.Interfaces;
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}