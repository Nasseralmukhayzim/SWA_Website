using System.Linq.Expressions;

namespace SWA.Application.Common.Interfaces;

/// <summary>Generic repository over an aggregate root. Query composition (filters, includes, paging) happens via the Queryable() escape hatch in query handlers; this surface covers the write side and simple reads.</summary>
public interface IRepository<T> where T : class
{
    IQueryable<T> Queryable();
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
