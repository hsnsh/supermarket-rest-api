using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public class CategoryDtoResponse : BaseDtoResponse<CategoryDto>
{
    public CategoryDtoResponse(CategoryDto resource) : base(resource)
    {
    }

    public CategoryDtoResponse(string message, bool isInternalError = false) : base(message, isInternalError)
    {
    }
}