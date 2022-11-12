using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class CategoryRepositoryTests
{
    private readonly DbContextOptions<CatalogServiceDbContext> _dbContextOptions;

    public CategoryRepositoryTests()
    {
        var dbName = $"CatalogServiceTestDb_{DateTime.Now.ToFileTimeUtc()}";
        _dbContextOptions = new DbContextOptionsBuilder<CatalogServiceDbContext>()
            .UseInMemoryDatabase(dbName).Options;
    }

    [Fact]
    public async Task GetListAsync_WithAllItems_ReturnsAllItems()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new CategoryRepository(context);
        var expectedCount = await context.Categories.CountAsync();

        // Act
        var actualItems = await repository.GetListAsync();

        // Assert
        var items = actualItems as Category[] ?? actualItems.ToArray();
        items.Should().BeOfType<Category[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);
    }

    private async Task<CatalogServiceTestDbContext> CreateDefaultContextAsync()
    {
        var context = new CatalogServiceTestDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync(); // Make sure the seeding process is finished!
        await PopulateDataAsync(context);
        return context;
    }

    private static async Task PopulateDataAsync(CatalogServiceDbContext context)
    {
        var index = 1;

        while (index <= 3)
        {
            var category = new Category() { Name = $"Populate Category {index}" };

            index++;
            await context.Categories.AddAsync(category);
        }

        await context.SaveChangesAsync();
    }
}