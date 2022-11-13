using System.Globalization;
using System.Linq.Expressions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Extensions;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class CategoryRepository : GenericRepository<CatalogServiceDbContext, Category>, ICategoryRepository
{
    public CategoryRepository(CatalogServiceDbContext dbContext) : base(dbContext)
    {
        DefaultPropertySelector = new List<Expression<Func<Category, object>>> { x => x.Products };
    }

    public async Task<List<Category>> GetPageListWithFiltersAsync(string filterText = null
        , string name = null
        , string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, bool includeDetails = false)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        var query = ApplyFilter(queryable, filterText, name);

        var sortingDesc = string.IsNullOrWhiteSpace(sorting) ? CategoryConsts.GetDefaultSorting(false) : sorting;

        return await query
            .OrderByIf(string.IsNullOrWhiteSpace(sortingDesc), sortingDesc)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetCountWithFiltersAsync(string filterText = null
        , string name = null)
    {
        var query = ApplyFilter(await GetQueryableAsync(), filterText, name);

        return await query.LongCountAsync();
    }

    protected virtual IQueryable<Category> ApplyFilter(IQueryable<Category> query, string filterText = null
        , string name = null)
    {
        filterText = filterText?.ToLower(new CultureInfo("tr-TR"));

        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.ToLower(new CultureInfo("tr-TR")).Contains(filterText))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.StartsWith(name));
    }
}