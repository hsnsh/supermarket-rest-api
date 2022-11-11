namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IList<Product> Products { get; set; } = new List<Product>();
}