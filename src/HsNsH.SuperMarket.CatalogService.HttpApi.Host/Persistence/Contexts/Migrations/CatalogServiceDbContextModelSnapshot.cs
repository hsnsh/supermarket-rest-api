﻿// <auto-generated />
using System;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HsNsH.SuperMarket.CatalogService.Persistence.Contexts.Migrations
{
    [DbContext(typeof(CatalogServiceDbContext))]
    partial class CatalogServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.11");

            modelBuilder.Entity("HsNsH.SuperMarket.CatalogService.Domain.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("HsNsH.SuperMarket.CatalogService.Domain.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT")
                        .HasColumnName("CategoryId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("Name");

                    b.Property<short>("QuantityInPackage")
                        .HasColumnType("INTEGER")
                        .HasColumnName("QuantityInPackage");

                    b.Property<byte>("UnitOfMeasurement")
                        .HasColumnType("INTEGER")
                        .HasColumnName("UnitOfMeasurement");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("HsNsH.SuperMarket.CatalogService.Domain.Models.Product", b =>
                {
                    b.HasOne("HsNsH.SuperMarket.CatalogService.Domain.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("HsNsH.SuperMarket.CatalogService.Domain.Models.Category", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
