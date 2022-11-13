using System.Linq.Expressions;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class GenericRepository<TDbContext, TEntity> : IGenericRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    protected readonly TDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true)
    {
        throw new Exception("Dead lock", new Exception("inner level 1 detail", new NotImplementedException()));
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(bool includeDetails = false)
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<List<TEntity>> GetPageListAsync(int maxResultCount = Int32.MaxValue, int skipCount = 0, bool includeDetails = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<long> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false)
    {
        throw new NotImplementedException();
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        throw new NotImplementedException();
    }
}