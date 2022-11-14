using HsNsH.SuperMarket.CatalogService.Domain;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Contexts.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(CatalogServiceDbProperties.DbTablePrefix + CategoryConsts.TableName, CatalogServiceDbProperties.DbSchema);
        builder.HasKey(ci => ci.Id);

        builder.Property(p => p.Id).IsRequired(); //.ValueGeneratedOnAdd();//.HasValueGenerator<InMemoryIntegerValueGenerator<int>>();
        builder.Property(p => p.Name).HasColumnName(nameof(Category.Name)).IsRequired().HasMaxLength(CategoryConsts.NameMaxLength);

        // builder.HasMany(p => p.Products)
        //     .WithOne(x=>x.Category)
        //     .HasForeignKey(p => p.CategoryId);

        // var navigation = builder.Metadata.FindNavigation(nameof(Category.Products));
        // navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}