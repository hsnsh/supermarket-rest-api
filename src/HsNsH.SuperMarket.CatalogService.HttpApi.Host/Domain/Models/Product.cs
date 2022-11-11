using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;

namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public short QuantityInPackage { get; set; }
    public EUnitOfMeasurement UnitOfMeasurement { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

}