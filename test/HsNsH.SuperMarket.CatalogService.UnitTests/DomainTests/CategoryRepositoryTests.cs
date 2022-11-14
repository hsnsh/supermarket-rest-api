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
        var actualPageItems = await repository.GetPageListWithFiltersAsync(includeDetails: true);
        var actualPageItemsCount = await repository.GetCountWithFiltersAsync();

        // Assert
        var items = actualPageItems as Category[] ?? actualPageItems.ToArray();
        items.Should().BeOfType<Category[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);
        actualPageItemsCount.Should().Be(expectedCount);

        // check include details
        items.Any(x => x.Products.Count > 0).Should().Be(true);
    }
}