using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace HsNsH.SuperMarket.CatalogService.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetListAsync(bool includeDetails = false);
    Task<IEnumerable<TEntity>> GetListAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = false);
    
    Task<TEntity> GetAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true);

    Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true);
}