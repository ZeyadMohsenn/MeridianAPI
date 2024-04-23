
using Microsoft.EntityFrameworkCore;
using StoreManagement.Bases;

namespace StoreManagement.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<T> GetRepository<T>() where T : BaseEntity<Guid>;
    Task<int> CommitAsync();
}
public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    TContext Context { get; }
}
