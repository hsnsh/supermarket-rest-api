using HsNsH.SuperMarket.CatalogService.Domain;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Contexts.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(CatalogServiceDbProperties.DbTablePrefix + ProductConsts.TableName, CatalogServiceDbProperties.DbSchema);
        builder.HasKey(ci => ci.Id);

        builder.Property(p => p.Id).IsRequired(); //.ValueGeneratedOnAdd();//.HasValueGenerator<InMemoryIntegerValueGenerator<int>>();
        builder.Property(p => p.Name).HasColumnName(nameof(Product.Name)).IsRequired().HasMaxLength(ProductConsts.NameMaxLength);
        builder.Property(p => p.QuantityInPackage).HasColumnName(nameof(Product.QuantityInPackage)).IsRequired();
        builder.Property(p => p.UnitOfMeasurement).HasColumnName(nameof(Product.UnitOfMeasurement)).IsRequired();

        builder.Property(x => x.CategoryId).HasColumnName(nameof(Product.CategoryId));

        builder.HasOne<Category>(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict); //no cascade delete
    }
}