using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;

public class CatalogServiceTestDbContext : CatalogServiceDbContext
{
    public CatalogServiceTestDbContext(DbContextOptions<CatalogServiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category() { Id = Guid.NewGuid(), Name = "Builder Category A" },
            new Category() { Id = Guid.NewGuid(), Name = "Builder Category B" }
        );

        SeedTestData<Category>(modelBuilder, "../../../DomainTests/TestBase/Categories.json");
    }

    private static void SeedTestData<T>(ModelBuilder modelBuilder, string file) where T : class
    {
        using var reader = new StreamReader(file);
        var json = reader.ReadToEnd();
        var data = JsonConvert.DeserializeObject<T[]>(json);
        if (data != null) modelBuilder.Entity<T>().HasData(data);
    }
}