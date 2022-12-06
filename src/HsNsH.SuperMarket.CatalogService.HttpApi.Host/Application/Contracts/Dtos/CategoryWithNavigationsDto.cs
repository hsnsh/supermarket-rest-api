namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class CategoryWithNavigationsDto : CategoryDto
{
    public ICollection<ProductDto> Products { get; set; }
}