using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;

public interface IProductAppService
{
    Task<List<ProductDto>> GetListAsync();
    Task<ProductDto> GetAsync(Guid id);

    Task<ProductDtoResponse> CreateAsync(ProductCreateDto input);

    Task<ProductDtoResponse> UpdateAsync(Guid id, ProductUpdateDto input);

    Task<BaseResponse> DeleteAsync(Guid id);
}