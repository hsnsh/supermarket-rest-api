using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;

namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class ProductDto : BaseDto<Guid>
{
    public string Name { get; set; }
    public short QuantityInPackage { get; set; }
    public EUnitOfMeasurement UnitOfMeasurement { get; set; }
    public Guid CategoryId { get; set; }
}