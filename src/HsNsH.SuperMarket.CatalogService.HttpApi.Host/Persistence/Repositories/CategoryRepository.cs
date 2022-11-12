using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class CategoryRepository : GenericRepository<CatalogServiceDbContext, Category>, ICategoryRepository
{
    public CategoryRepository(CatalogServiceDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Category>> GetPageListWithFiltersAsync(string filterText = null, string name = null, string sorting = null, int maxResultCount = Int32.MaxValue, int skipCount = 0, bool includeDetails = false)
    {
        throw new NotImplementedException();
    }

    public async Task<long> GetCountWithFiltersAsync(string filterText = null, string name = null)
    {
        throw new NotImplementedException();
    }
}