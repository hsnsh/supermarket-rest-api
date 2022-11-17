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

    public GenericRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        return await queryable.Where(predicate).SingleOrDefaultAsync();
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

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(string sorting = null, bool includeDetails = false)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        return await queryable
            .OrderByIf<TEntity, IQueryable<TEntity>>(!string.IsNullOrWhiteSpace(sorting), sorting)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, string sorting = null, bool includeDetails = false)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        return await queryable
            .Where(predicate)
            .OrderByIf<TEntity, IQueryable<TEntity>>(!string.IsNullOrWhiteSpace(sorting), sorting)
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetPageListAsync(int skipCount, int maxResultCount, string sorting = null, bool includeDetails = false)
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
            await GetDbContext().SaveChangesAsync();
        }

        return savedEntity;
    }

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        var entityArray = entities.ToArray();

        await GetDbSet().AddRangeAsync(entityArray);

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync();
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false)
    {
        var dbContext = GetDbContext();
        dbContext.Attach(entity);

        var updatedEntity = dbContext.Update(entity).Entity;

        if (autoSave)
        {
            await dbContext.SaveChangesAsync();
        }

        return updatedEntity;
    }

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        GetDbSet().UpdateRange(entities);

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false)
    {
        GetDbSet().Remove(entity);

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false)
    {
        var entities = await GetDbSet().Where(predicate).ToListAsync();

        await DeleteManyAsync(entities, autoSave);

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync();
        }
    }

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false)
    {
        GetDbContext().RemoveRange(entities);

        if (autoSave)
        {
            await GetDbContext().SaveChangesAsync();
        }
    }


    protected TDbContext GetDbContext()
    {
        return _dbContext;
    }

    protected DbSet<TEntity> GetDbSet()
    {
        return GetDbContext().Set<TEntity>();
    }

    protected async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return GetDbSet().AsQueryable();
    }

    protected async Task<IQueryable<TEntity>> WithDetailsAsync()
    {
        if (DefaultPropertySelector == null)
        {
            return await GetQueryableAsync();
        }

        return await WithDetailsAsync(DefaultPropertySelector?.ToArray());
    }

    protected async Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return IncludeDetails(await GetQueryableAsync(), propertySelectors);
    }

    private static IQueryable<TEntity> IncludeDetails(IQueryable<TEntity> query, IReadOnlyCollection<Expression<Func<TEntity, object>>> propertySelectors)
    {
        return propertySelectors is not { Count: > 0 } ? query : propertySelectors.Aggregate(query, (current, propertySelector) => current.Include(propertySelector));
    }
}