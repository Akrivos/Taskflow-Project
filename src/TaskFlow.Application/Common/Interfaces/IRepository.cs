using System.Linq.Expressions;

namespace TaskFlow.Application.Common.Interfaces;
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}