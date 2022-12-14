using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public abstract class BaseRepositoryTests
{
    protected static async Task<CatalogServiceTestDbContext> CreateDefaultContextAsync()
    {
        var dbName = $"TestDb_{DateTime.Now.ToFileTimeUtc()}_{Guid.NewGuid().ToString("N")}";
        var dbContextOptions = new DbContextOptionsBuilder<CatalogServiceDbContext>()
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseInMemoryDatabase(dbName).Options;

        var context = new CatalogServiceTestDbContext(dbContextOptions);
        await context.Database.EnsureCreatedAsync(); // Make sure the seeding process is finished!
        await PopulateDataAsync(context);
        return context;
    }

    private static async Task PopulateDataAsync(CatalogServiceDbContext context)
    {
        var categories = new List<Category>()
        {
            new() { Name = "AAA Category" },
            new() { Name = "ZZZ Category" },
            new() { Name = "Populate Category A" },
            new() { Name = "Populate Category B" },
            new() { Name = "Populate Category C" },
            new() { Name = "Populate Category D" },
            new() { Name = "Populate Category E" },
            new(Guid.Parse("a03cf65c-edfc-4a23-90a8-112fd957fa5a")) { Name = "Populate Category Test" },
        };
        await context.Categories.AddRangeAsync(categories);

        var index = 1;

        while (index <= 25)
        {
            var product = new Product { CategoryId = categories[Random.Shared.Next(categories.Count)].Id, Name = $"Populate Product {index}", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Unity };

            index++;
            await context.Products.AddAsync(product);
        }

        var testProduct = new Product(Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5")) { CategoryId = Guid.Parse("a03cf65c-edfc-4a23-90a8-112fd957fa5a"), Name = $"Populate Product Test", QuantityInPackage = 100, UnitOfMeasurement = EUnitOfMeasurement.Unity };
        await context.Products.AddAsync(testProduct);

        await context.SaveChangesAsync();
    }
}