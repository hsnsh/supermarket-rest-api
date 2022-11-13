using System.Linq.Expressions;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class GenericRepository<TDbContext, TEntity> : IGenericRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly TDbContext _dbContext;
    protected List<Expression<Func<TEntity, object>>> DefaultPropertySelector = null;

    protected GenericRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected TDbContext GetDbContext()
    {
        return _dbContext;
    }

    protected DbSet<TEntity> GetDbSet()
    {
        return GetDbContext().Set<TEntity>();
    }

    public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true)
    {
        return await GetDbSet().Where(predicate).SingleOrDefaultAsync();
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true)
    {
        var entity = await FindAsync(predicate, includeDetails);

        if (entity == null)
        {
            throw new DomainException($"There is no such an entity. Entity type: {typeof(TEntity).FullName}");
        }

        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(bool includeDetails = false)
    {
        return await GetDbSet().ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false)
    {
        return await GetDbSet().Where(predicate).ToListAsync();
    }

    public virtual async Task<List<TEntity>> GetPageListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        return await queryable
            .OrderByIf<TEntity, IQueryable<TEntity>>(!string.IsNullOrWhiteSpace(sorting), sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public virtual async Task<long> GetCountAsync()
    {
        return await GetDbSet().LongCountAsync();
    }

    public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false)
    {
        var savedEntity = (await GetDbSet().AddAsync(entity)).Entity;

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }

        return savedEntity;
    }

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        await GetDbSet().AddRangeAsync(entities);

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
    {
        _dbContext.Attach(entity);

        var updatedEntity = _dbContext.Update(entity).Entity;

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }

        return updatedEntity;
    }

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        GetDbSet().UpdateRange(entities);

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
    {
        var entities = await GetListAsync(predicate, includeDetails: false);

        await DeleteManyAsync(entities, autoSave);

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false)
    {
        GetDbSet().Remove(entity);

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        _dbContext.RemoveRange(entities);

        if (autoSave)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    protected async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return GetDbSet().AsQueryable();
    }

    public async Task<IQueryable<TEntity>> WithDetailsAsync()
    {
        if (DefaultPropertySelector == null)
        {
            return await GetQueryableAsync();
        }

        return await WithDetailsAsync(DefaultPropertySelector?.ToArray());
    }

    public async Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return IncludeDetails(await GetQueryableAsync(), propertySelectors);
    }

    private IQueryable<TEntity> IncludeDetails(IQueryable<TEntity> query, IReadOnlyCollection<Expression<Func<TEntity, object>>> propertySelectors)
    {
        if (propertySelectors != null && propertySelectors.Count > 0)
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }

        return query;
    }
}