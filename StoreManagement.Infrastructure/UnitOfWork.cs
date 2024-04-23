using StoreManagement.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Bases;

namespace StoreManagement.Infrastructure
{
     
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IUnitOfWork where TContext : DbContext
    {
        private Dictionary<Type, object> _repositories;
        public TContext Context { get; }

        public UnitOfWork(TContext context)
        {
            Context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            Context.Dispose();
           _repositories.Clear();
           GC.SuppressFinalize(this);
        }

        public IBaseRepository<T> GetRepository<T>() where T : BaseEntity<Guid>
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new BaseRepository<T, TContext>(Context);
            return (IBaseRepository<T>)_repositories[type];
        }

        public Task<int> CommitAsync()
        {
            return  Context.SaveChangesAsync();
        }
    }
}