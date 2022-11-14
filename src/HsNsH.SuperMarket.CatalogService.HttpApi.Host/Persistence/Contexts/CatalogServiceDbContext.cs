using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Contexts;

public class CatalogServiceDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    public CatalogServiceDbContext(DbContextOptions<CatalogServiceDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // load all configuration class => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}