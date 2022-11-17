using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;

namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

public class Product : Entity<Guid>
{
    public Product() : base(Guid.NewGuid())
    {
    }

    public Product(Guid id) : base(id)
    {
    }

    public string Name { get; set; }
    public short QuantityInPackage { get; set; }
    public EUnitOfMeasurement UnitOfMeasurement { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }
}