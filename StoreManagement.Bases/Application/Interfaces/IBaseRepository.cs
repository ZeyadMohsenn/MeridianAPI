using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StoreManagement.Bases
{
    public interface IBaseRepository<TEntity>
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entites);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entites);
        void Update(TEntity entity);
        TEntity? FindByID(Guid Id);
        Task<TEntity> FindByIdAsync(Guid Id);
        IQueryable<TEntity> GetAllQueryableAsync();

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filterPredicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
            object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TEntity>> select = null, int? take = null, bool ignoreFilter = false);
        Task<List<TEntity>> GetAllAsyncWithTracking(Expression<Func<TEntity, bool>> filterPredicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
            object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, TEntity>> select = null, int? take = null, bool ignoreFilter = false);
        Task<IQueryable<TEntity>> GetAllQueryableAsync(Expression<Func<TEntity, bool>>? filterPredicate=null,
                                                       Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null,
                                                       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                       bool withDeleted = false);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterPredicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
                object>> Include = null, bool asNoTracking = false, Expression<Func<TEntity, TEntity>> select = null, bool asSplit = false,
            bool IgnoreFilter = false,
            bool withDeleted = false);
        Task<bool> IsExists(Expression<Func<TEntity, bool>> filterPredicate = null);
        void Delete(TEntity entity);
        Task<(List<TEntity>, int)> GetPaginatedAsync(Expression<Func<TEntity, bool>> filterPredicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //Pagination pagentation = null,
            Expression<Func<TEntity, TEntity>> select = null);

        IQueryable<TEntity> GetPaginatedQuerableAsync(Expression<Func<TEntity, bool>> filterPredicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //  Pagination pagentation = null,
            Expression<Func<TEntity, TEntity>> select = null, bool withDelted = false);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filterPredicate = null, bool withDeleted = false);

    }

}
