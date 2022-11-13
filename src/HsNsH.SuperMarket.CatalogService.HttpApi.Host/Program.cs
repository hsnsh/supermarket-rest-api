using System.Diagnostics.CodeAnalysis;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService;

public static class Program
{
    [ExcludeFromCodeCoverage]
    public static async Task<int> Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogServiceDbContext>();
            await context.Database.MigrateAsync();

            var categories = new List<Category>()
            {
                new() { Id = Guid.NewGuid(), Name = "Default Category A" },
                new() { Id = Guid.NewGuid(), Name = "Default Category B" },
                new() { Id = Guid.NewGuid(), Name = "Default Category C" },
                new() { Id = Guid.NewGuid(), Name = "Default Category D" },
                new() { Id = Guid.NewGuid(), Name = "Default Category E" },
            };
            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                await context.Products.AddRangeAsync(new List<Product>()
                {
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product A", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Unity },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product B", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Liter },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product C", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Milligram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product D", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Gram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product E", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Kilogram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product F", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Unity },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product G", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Liter },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product H", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Milligram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product I", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Gram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product J", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Kilogram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product K", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Unity },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product Q", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Liter },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product W", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Milligram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product X", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Gram },
                    new() { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = "Default Product Z", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Kilogram },
                });
                await context.SaveChangesAsync();
            }
        }

        await host.RunAsync();

        return 0;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}