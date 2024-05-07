using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data;

namespace StoreManagement.Bases;

public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity> where TEntity : BaseEntity<Guid> where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    public BaseRepository(TContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    public Task AddAsync(TEntity entity)
    {
        return Task.FromResult(_dbSet.AddAsync(entity));
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return _dbSet.AddRangeAsync(entities);
    }

    public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterPredicate,
                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null,
                                    bool asNoTracking = false,
                                    Expression<Func<TEntity, TEntity>> select = null,
                                    bool asSplit = false,
                                    bool ignoreFilter = false,
                                    bool withDeleted = false)
    {
        var query = _dbSet.AsQueryable();
        if (!withDeleted)
            query = query.Where(a => !a.Is_Deleted);

        query = query.Where(filterPredicate);

        if (ignoreFilter)
            query = query.IgnoreQueryFilters();

        if (Include is not null)
            query = Include(query);
        if (asNoTracking)
            query = query.AsNoTracking();
        if (select is not null)
            query = query.Select(select);
        if (asSplit)
            query = query.AsSplitQuery();
        return query.FirstOrDefaultAsync();

    }

    public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filterPredicate = null,
                                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null,
                                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                            Expression<Func<TEntity, TEntity>> select = null,
                                            int? take = null,
                                            bool ignoreFilter = false)
    {
        var query = _dbSet.AsQueryable();


        query = query.Where(a => !a.Is_Deleted).Where(filterPredicate);


        if (ignoreFilter)
            query = query.IgnoreQueryFilters();
        if (Include is not null)
            query = Include(query);
        if (orderBy is not null)
            query = orderBy(query);
        if (take is not null)
            query = query.Take(take.Value);
        if (select is not null)
            query = query.Select(select);


        return query.AsNoTrackingWithIdentityResolution().ToListAsync();
    }

    public Task<List<TEntity>> GetAllAsyncWithTracking(Expression<Func<TEntity, bool>> filterPredicate,
                                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null,
                                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                        Expression<Func<TEntity, TEntity>> select = null,
                                                        int? take = null,
                                                        bool ignoreFilter = false)
    {
        var query = _dbSet.AsQueryable();


        query = query.Where(a => !a.Is_Deleted).Where(filterPredicate);


        if (ignoreFilter)
            query = query.IgnoreQueryFilters();
        if (Include is not null)
            query = Include(query);
        if (orderBy is not null)
            query = orderBy(query);
        if (take is not null)
            query = query.Take(take.Value);
        if (select is not null)
            query = query.Select(select);


        return query.ToListAsync();
    }

    public void Remove(TEntity entity)
    {

        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }
    public void Delete(TEntity entity)
    {
        entity.Is_Deleted = true;
    }

    public Task<bool> IsExists(Expression<Func<TEntity, bool>> filterPredicate = null)
    {
        var query = _dbSet.Where(a => !a.Is_Deleted);

        if (filterPredicate is not null)
            return query.AsNoTracking().AnyAsync(filterPredicate);

        return query.AsNoTracking().AnyAsync();
    }

    public Task<List<TEntity>> GetAllOrderesAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        var query = _dbSet.AsQueryable();

        if (orderBy is not null)
            query = orderBy(query);

        return query.Where(a => !a.Is_Deleted).ToListAsync();
    }

    public async Task<(List<TEntity>, int)> GetPaginatedAsync(Expression<Func<TEntity, bool>> filterPredicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //Pagination pagentation = null,
        Expression<Func<TEntity, TEntity>> select = null)
    {

        int count = 0;
        var query = _dbSet.AsNoTracking().AsQueryable();
        query = query.Where(a => !a.Is_Deleted);
        if (Include is not null)
            query = Include(query);
        if (orderBy is not null)
            query = orderBy(query);

        if (filterPredicate != null)
            query = query.Where(filterPredicate);
        if (select is not null)
            query = query.Select(select);
        //if (pagentation is not null)
        //{
        //    count = await query.CountAsync();
        //    query = query.Skip((pagentation.PageNumber - 1) * pagentation.PageSize).Take(pagentation.PageSize);
        //}
        return (await query.ToListAsync(), count);
    }

    public async Task<IQueryable<TEntity>> GetAllQueryableAsync(Expression<Func<TEntity, bool>> filterPredicate = null, Func<IQueryable<TEntity>,
        IIncludableQueryable<TEntity, object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool withDeleted = false)
    {
        var query = _dbSet.AsQueryable();

        if (Include is not null)
            query = Include(query);
        if (orderBy is not null)
            query = orderBy(query);

        if (withDeleted)
            return await Task.FromResult(query.Where(filterPredicate));


        return await Task.FromResult(query.Where(a => !a.Is_Deleted).Where(filterPredicate));
    }

    //public asyncIQueryable<TEntity> GetPaginatedQuerableAsync( )
    //{

    //    var query = _dbSet.AsQueryable().Where(a => !a.Is_Deleted);

    //    return query;
    //}
    public IQueryable<TEntity> GetAllQueryableAsync()
    {
        return _dbSet.AsQueryable().Where(a => !a.Is_Deleted);
    }

    public IQueryable<TEntity> GetPaginatedQuerableAsync(Expression<Func<TEntity, bool>> filterPredicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
        object>> Include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //Pagination pagentation = null,
        Expression<Func<TEntity, TEntity>> select = null,
        bool withDeleted = false)
    {

        var query = _dbSet.AsQueryable();
        if (!withDeleted)
            query = query.Where(a => !a.Is_Deleted);

        if (Include is not null)
            query = Include(query);
        if (orderBy is not null)
            query = orderBy(query);

        if (filterPredicate != null)
            query = query.Where(filterPredicate);
        if (select is not null)
            query = query.Select(select);
        //if (pagentation is not null)
        //{

        //    query = query.Skip((pagentation.PageNumber - 1) * pagentation.PageSize).Take(pagentation.PageSize);
        //}
        return query;
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> filterPredicate = null, bool withDeleted = false)
    {

        var query = _dbSet.AsQueryable().AsNoTracking();

        if (!withDeleted)
            query = query.Where(a => !a.Is_Deleted);


        if (filterPredicate is not null)
            query = query.Where(filterPredicate);

        return query.CountAsync();
    }

    public TEntity? FindByID(Guid Id)
    {
        return _dbContext.Set<TEntity>().Where(a => !a.Is_Deleted).FirstOrDefault(z => z.Id.Equals(Id));
    }

    public async Task<TEntity> FindByIdAsync(Guid Id)
    {
        return await Task.FromResult(_dbContext.Set<TEntity>().Where(a => !a.Is_Deleted).FirstOrDefault(z => z.Id.Equals(Id)));
    }
}

//public class BaseRepository<T, YDbContext, IdType> : IBaseRepository<T, IdType> where T : BaseEntity<IdType> where YDbContext : DbContext
//{
//    private static bool _hasIsDeleted = typeof(T).GetType() is BaseCommonEntity<IdType>;
//    protected readonly YDbContext _dbContext;

//    protected readonly Expression<Func<T, bool>> ignoreDeleted = z => _hasIsDeleted ? (z as BaseCommonEntity<IdType>).Is_Deleted == false : true;

//    public BaseRepository(YDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
//    {
//        if (predicate == null)
//            return await _dbContext.Set<T>().Where(ignoreDeleted).ToListAsync();
//        else
//            return await _dbContext.Set<T>().Where(ignoreDeleted).Where(predicate).ToListAsync();
//    }

//    public virtual List<T> GetAll(Expression<Func<T, bool>> predicate = null)
//    {
//        if (predicate == null)
//            return _dbContext.Set<T>().Where(ignoreDeleted).ToList();
//        else
//            return _dbContext.Set<T>().Where(ignoreDeleted).Where(predicate).ToList();
//    }

//    public virtual bool HasAny(Expression<Func<T, bool>> predicate = null)
//    {
//        return _dbContext.Set<T>().Where(ignoreDeleted).Any(predicate);
//    }

//    public virtual async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null)
//    {
//        return await _dbContext.Set<T>().Where(ignoreDeleted).CountAsync(predicate);
//    }

//    public virtual IQueryable<T> GetAll()
//    {
//        return _dbContext.Set<T>().AsQueryable();
//    }

//    public virtual IQueryable<T> GetAllDescendingAsync<Y>(Expression<Func<T, Y>> sort=null, Expression<Func<T, bool>> predicate = null)
//    {
//        if (predicate == null)
//            return _dbContext.Set<T>().Where(ignoreDeleted).OrderByDescending(sort).AsQueryable();
//        else
//            return _dbContext.Set<T>().Where(ignoreDeleted).Where(predicate).OrderByDescending(sort).AsQueryable();
//    }

//    public virtual IQueryable<T> GetAllAscendingAsync<Y>(Expression<Func<T, Y>> sort, Expression<Func<T, bool>> predicate = null)
//    {
//        if (predicate == null)
//            return _dbContext.Set<T>().Where(ignoreDeleted).OrderBy(sort).AsQueryable();
//        else
//            return _dbContext.Set<T>().Where(ignoreDeleted).Where(predicate).OrderBy(sort).AsQueryable();
//    }

//    public virtual T FindByID(IdType Id)
//    {
//        return _dbContext.Set<T>().Where(ignoreDeleted).FirstOrDefault(z => z.Id.Equals(Id));
//    }
//    public virtual T FindFirst(Expression<Func<T, bool>> predicate = null)
//    {
//        return _dbContext.Set<T>().Where(ignoreDeleted).FirstOrDefault(predicate);
//    }

//    public IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> predicate = null)
//    {
//        var values= _dbContext.Set<T>().Where(ignoreDeleted).Where(predicate);
//        return values;
//    }
//}

//public class ModifyBaseRepository<T, YDbContext, IdType> : BaseRepository<T, YDbContext, IdType>, IModifyBaseRepository<T, IdType> where T : BaseEntity<IdType> where YDbContext : DbContext
//{
//    public ModifyBaseRepository(YDbContext dbContext):base(dbContext)
//    {

//    }

//    public virtual void Create(T obj)
//    {
//        _dbContext.Attach(obj);
//        _dbContext.Entry(obj).State = EntityState.Added;
//    }


//    public virtual void CreateRange(IEnumerable<T> objs)
//    {
//        _dbContext.Set<T>().AddRangeAsync(objs);
//    }

//    public void Delete(params IdType[] idList)
//    {
//        _dbContext.Set<T>().Where(z => idList.Contains(z.Id)).ForEachAsync(delegate (T item)
//        {
//            //item.is = true;
//        });
//    }

//    public void PhysicalDelete(T obj)
//    {
//        _dbContext.Set<T>().Remove(obj);
//    }

//    public void Update(T obj, params string[] excludedFields)
//    {
//        _dbContext.Entry(obj).State = EntityState.Modified;
//        foreach (string fld in excludedFields)
//            _dbContext.Entry(obj).Property(fld).IsModified = false;
//    }

//    public void UpdateField<C>(Expression<Func<T, bool>> predicate, string columnName, C value)
//    {
//        throw new NotImplementedException();
//    }
//}

