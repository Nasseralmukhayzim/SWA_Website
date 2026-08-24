using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Interfaces;

namespace SWA.Infrastructure.Persistence.Repositories;

public sealed class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : class
{
    private readonly DbSet<T> _set = dbContext.Set<T>();

    public IQueryable<T> Queryable() => _set.AsQueryable();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _set.FindAsync([id], cancellationToken).AsTask();

    public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) =>
        _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken) =>
        await _set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
