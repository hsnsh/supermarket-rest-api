using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public class CategoryDtoResponse : BaseDtoResponse<CategoryDto>
{
    public CategoryDtoResponse(CategoryDto resource) : base(resource)
    {
    }

    public CategoryDtoResponse(string message, int code = 0) : base(message, code)
    {
    }

    public CategoryDtoResponse(IReadOnlyCollection<string> messages, int code = 0) : base(messages, code)
    {
    }
}