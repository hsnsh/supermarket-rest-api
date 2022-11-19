using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;

public interface IDataAppService
{
    Task<List<CategoryDto>> GetListAsync();
    Task<List<CategoryWithNavigationsDto>> GetListWithNavigationsAsync();
}