namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class CategoryDto : BaseDto<Guid>
{
    public string Name { get; set; }
}