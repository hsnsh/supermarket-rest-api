using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class CategoryRepositoryTests : BaseRepositoryTests
{
    [Fact]
    public async Task GetPageListWithFiltersAsync_WithNoFilter_ReturnsAllItems()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new CategoryRepository(context);
        var expectedCount = await context.Categories.CountAsync();

        // Act
        var actualItems = await repository.GetPageListWithFiltersAsync(includeDetails: true);

        // Assert
        var items = actualItems as Category[] ?? actualItems.ToArray();
        items.Should().BeOfType<Category[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);

        // check include details
        items.Any(x => x.Products.Count > 0).Should().Be(true);
    }
}