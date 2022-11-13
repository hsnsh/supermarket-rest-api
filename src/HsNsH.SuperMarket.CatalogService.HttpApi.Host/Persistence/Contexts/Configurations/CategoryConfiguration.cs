using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Contexts.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(CategoryConsts.TableName);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired(); //.ValueGeneratedOnAdd();//.HasValueGenerator<InMemoryIntegerValueGenerator<int>>();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(CategoryConsts.NameMaxLength);
        builder.HasMany(p => p.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
    }
}