using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Abstractions;

public class PagedResult<T>(List<T> items, int totalCount, int pageNumber, int pageSize)
{
    public IReadOnlyList<T> Items { get; set; } = items;
    public int TotalCount { get; set; } = totalCount;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}

public interface IRepository<T> where T : Entity
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    Task MarkForAddAsync(T entity, CancellationToken cancellationToken = default);
    Task MarkForUpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task MarkForDeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> MarkForDeleteWhereAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties);

    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties);

    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties);

    Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties);
}