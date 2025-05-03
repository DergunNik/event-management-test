using System.Linq.Expressions;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EFCore;

public class EfcRepository<T>(AppDbContext context) : IRepository<T> where T : Entity
{
    private readonly DbSet<T> _entities = context.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return await _entities.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T> query = _entities;

        query = includesProperties?.Aggregate(query,
            (current, include) => current.Include(include)) ?? query;

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable().Where(x => x.Id == id);

        query = includesProperties?.Aggregate(query,
            (current, include) => current.Include(include)) ?? query;

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        var query = GetQueryable(filter, includesProperties);

        if (orderBy is not null) query = orderBy(query);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        var query = GetQueryable(filter, includesProperties);
        return await query.ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.AnyAsync(filter, cancellationToken);
    }

    private IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter,
        Expression<Func<T, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable();

        if (filter is not null) query = query.Where(filter);

        query = includesProperties?.Aggregate(query,
            (current, include) => current.Include(include)) ?? query;

        return query;
    }
}