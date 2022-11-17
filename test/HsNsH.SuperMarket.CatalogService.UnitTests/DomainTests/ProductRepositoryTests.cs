using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class ProductRepositoryTests : BaseRepositoryTests
{
    [Fact]
    public async Task GetPageListWithFiltersAsync_WithNoFilter_ReturnsAllItems()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new ProductRepository(context);
        var expectedCount = await context.Products.CountAsync();

        // Act
        var actualPageItems = await repository.GetPageListWithFiltersAsync(includeDetails: true);
        var actualPageItemsCount = await repository.GetCountWithFiltersAsync();

        // Assert
        var items = actualPageItems as Product[] ?? actualPageItems.ToArray();
        items.Should().BeOfType<Product[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);
        actualPageItemsCount.Should().Be(expectedCount);

        // check include details
        items.Any(x => x.Category != null).Should().Be(true);
    }
}