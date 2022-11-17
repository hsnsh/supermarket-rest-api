using System.Globalization;
using System.Linq.Expressions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Extensions;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class ProductRepository : GenericRepository<CatalogServiceDbContext, Product>, IProductRepository
{
    public ProductRepository(CatalogServiceDbContext dbContext) : base(dbContext)
    {
        DefaultPropertySelector = new List<Expression<Func<Product, object>>> { x => x.Category };
    }

    public async Task<IEnumerable<Product>> GetPageListWithFiltersAsync(string filterText = null
        , string name = null
        , EUnitOfMeasurement? unitOfMeasurement = null
        , Guid? categoryId = null
        , string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, bool includeDetails = false)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetQueryableAsync();

        var query = ApplyFilter(queryable, filterText, name, unitOfMeasurement, categoryId);

        var sortingDesc = string.IsNullOrWhiteSpace(sorting) ? ProductConsts.GetDefaultSorting() : sorting;

        return await query
            .OrderByIf(string.IsNullOrWhiteSpace(sortingDesc), sortingDesc)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<long> GetCountWithFiltersAsync(string filterText = null
        , string name = null
        , EUnitOfMeasurement? unitOfMeasurement = null
        , Guid? categoryId = null)
    {
        var query = ApplyFilter(await GetQueryableAsync(), filterText, name, unitOfMeasurement, categoryId);

        return await query.LongCountAsync();
    }

    protected virtual IQueryable<Product> ApplyFilter(IQueryable<Product> query, string filterText = null
        , string name = null
        , EUnitOfMeasurement? unitOfMeasurement = null
        , Guid? categoryId = null)
    {
        filterText = filterText?.ToLower(new CultureInfo("tr-TR"));

        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.ToLower(new CultureInfo("tr-TR")).Contains(filterText))
            .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.StartsWith(name))
            .WhereIf(unitOfMeasurement.HasValue, e => e.UnitOfMeasurement == unitOfMeasurement.Value)
            .WhereIf(categoryId.HasValue, e => e.CategoryId == categoryId.Value);
    }
}