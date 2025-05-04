using System.Collections.Concurrent;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EFCore;

public class EfcUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfcUnitOfWork> _logger;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

    public EfcUnitOfWork(AppDbContext context, ILogger<EfcUnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IRepository<T> GetRepository<T>() where T : Entity
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new EfcRepository<T>(_context));
    }

    public async Task CreateDataBaseAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DeleteDataBaseAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void BeginTransaction()
    {
        _currentTransaction = _context.Database.BeginTransaction();
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
                _logger.LogError("Commit transaction error: {err}", e.Message);
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
                _logger.LogError("Rollback transaction error: {err}", e.Message);
                throw;
            }
            finally
            {
                _currentTransaction = null;
            }
    }
}