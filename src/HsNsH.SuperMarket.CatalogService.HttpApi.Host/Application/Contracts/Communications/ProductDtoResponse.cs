using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public class ProductDtoResponse : BaseDtoResponse<ProductDto>
{
    public ProductDtoResponse(ProductDto resource) : base(resource)
    {
    }

    public ProductDtoResponse(string message, bool isInternalError = false) : base(message, isInternalError)
    {
    }
}