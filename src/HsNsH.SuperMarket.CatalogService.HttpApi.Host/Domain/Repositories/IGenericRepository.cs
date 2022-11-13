using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HsNsH.SuperMarket.CatalogService.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true);
    Task<TEntity> GetAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true);

    Task<IEnumerable<TEntity>> GetListAsync(bool includeDetails = false);
    Task<IEnumerable<TEntity>> GetListAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = false);

    Task<List<TEntity>> GetPageListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false);
    Task<long> GetCountAsync();

    Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false);
    Task InsertManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false);

    Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false);
    Task UpdateManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false);

    Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false);
    Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false);
    Task DeleteManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false);
}