using Domain.Entities;

namespace Domain.Abstractions;

public interface IUnitOfWork
{
    IRepository<T> GetRepository<T>() where T : Entity;

    Task SaveChangesAsync();

    void BeginTransaction();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    Task DeleteDataBaseAsync();

    Task CreateDataBaseAsync();
}