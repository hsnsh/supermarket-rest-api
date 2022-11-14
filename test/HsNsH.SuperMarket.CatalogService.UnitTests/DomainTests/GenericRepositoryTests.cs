using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class GenericRepositoryTests : BaseRepositoryTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData($"{nameof(Product.Name)} desc", false)]
    [InlineData($"{nameof(Product.Name)} asc", false)]
    public async Task GetListAsync_WithAllItems_ReturnsAllItems(string sorting, bool includeDetails)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var expectedCount = await context.Products.CountAsync();

        // Act
        var actualItems = await repository.GetListAsync(sorting: sorting, includeDetails: includeDetails);

        // Assert
        var items = actualItems as Product[] ?? actualItems.ToArray();
        items.Should().BeOfType<Product[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);

        if (!string.IsNullOrWhiteSpace(sorting))
        {
            var expectedAscItemName = await context.Products.OrderBy(x => x.Name).Select(x => x.Name).FirstAsync();
            var expectedDescItemName = await context.Products.OrderByDescending(x => x.Name).Select(x => x.Name).FirstAsync();
            items[0].Name.Should().Be(sorting.Split(' ')[1].Equals("desc") ? expectedDescItemName : expectedAscItemName);
        }

        if (includeDetails)
        {
            items.Any(x => x.Category != null).Should().Be(true);
        }
    }
}