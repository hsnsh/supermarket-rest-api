using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;

namespace HsNsH.SuperMarket.CatalogService.Application.Services;

public class CategoryAppService : ICategoryAppService
{
    public async Task<List<CategoryDto>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDtoResponse> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDtoResponse> CreateAsync(CategoryCreateDto input)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDtoResponse> UpdateAsync(Guid id, CategoryUpdateDto input)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}