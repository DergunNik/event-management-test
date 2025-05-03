using System.Collections.Concurrent;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EFCore;

public class EfcUnitOfWork(AppDbContext context, ILogger<EfcUnitOfWork> logger) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

    public IRepository<T> GetRepository<T>() where T : Entity
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new EfcRepository<T>(context));
    }

    public async Task CreateDataBaseAsync()
    {
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DeleteDataBaseAsync()
    {
        await context.Database.EnsureDeletedAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public void BeginTransaction()
    {
        _currentTransaction = context.Database.BeginTransaction();
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction != null)
            try
            {
                await _currentTransaction.CommitAsync();
            }
            catch (Exception e)
            {
                logger.LogError("Commit transaction error: {err}", e.Message);
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _currentTransaction = null;
            }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
            try
            {
                await _currentTransaction.RollbackAsync();
            }
            catch (Exception e)
            {
                logger.LogError("Rollback transaction error: {err}", e.Message);
                throw;
            }
            finally
            {
                _currentTransaction = null;
            }
    }
}