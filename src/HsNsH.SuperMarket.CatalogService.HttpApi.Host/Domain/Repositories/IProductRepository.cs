using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;

namespace HsNsH.SuperMarket.CatalogService.Domain.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetPageListWithFiltersAsync(string filterText = null
        , string name = null
        , EUnitOfMeasurement? unitOfMeasurement = null
        , Guid? categoryId = null
        , string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0
        , bool includeDetails = false);

    Task<long> GetCountWithFiltersAsync(string filterText = null
        , string name = null
        , EUnitOfMeasurement? unitOfMeasurement = null
        , Guid? categoryId = null
    );
}