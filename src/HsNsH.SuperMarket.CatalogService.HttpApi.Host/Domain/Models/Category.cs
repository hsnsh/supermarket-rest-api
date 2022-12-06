namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

public class Category : Entity<Guid>
{
    public Category() : base(Guid.NewGuid())
    {
    }

    public Category(Guid id) : base(id)
    {
    }

    public string Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
}