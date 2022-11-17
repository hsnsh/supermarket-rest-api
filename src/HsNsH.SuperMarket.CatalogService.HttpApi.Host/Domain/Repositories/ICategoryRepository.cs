using HsNsH.SuperMarket.CatalogService.Domain.Models;

namespace HsNsH.SuperMarket.CatalogService.Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetPageListWithFiltersAsync(string filterText = null
        , string name = null
        , string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0
        , bool includeDetails = false);

    Task<long> GetCountWithFiltersAsync(string filterText = null
        , string name = null
    );
}