namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class ProductWithNavigationsDto : ProductDto
{
    public CategoryDto Category { get; set; }
}