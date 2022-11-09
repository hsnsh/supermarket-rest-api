using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;

public interface ICategoryAppService
{
    Task<List<CategoryDto>> GetListAsync();
    Task<CategoryDtoResponse> GetByIdAsync(Guid id);

    Task<CategoryDtoResponse> CreateAsync(CategoryCreateDto input);

    Task<CategoryDtoResponse> UpdateAsync(Guid id, CategoryUpdateDto input);

    Task<BaseResponse> DeleteAsync(Guid id);
}