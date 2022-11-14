using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class GenericRepositoryTests : BaseRepositoryTests
{
    #region GetListAsync_Tests

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

    #endregion

    #region FindAsync_Tests

    [Fact]
    public async Task FindAsync_WithUnExistingItem_ReturnsNull()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var refId = Guid.NewGuid();

        // Act
        var result = await repository.FindAsync(x => x.Id.Equals(refId), includeDetails: false);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindAsync_WithDuplicateItems_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.FindAsync(x => x.Name.Contains("Product"), includeDetails: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().Be("Sequence contains more than one element");
    }

    [Theory]
    [InlineData("b61cf65c-edfc-4a23-90a8-112fd957fab5", true)]
    [InlineData("b61cf65c-edfc-4a23-90a8-112fd957fab5", false)]
    public async Task FindAsync_WithExistingItem_ReturnsExpectedItem(string findId, bool includeDetails)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var refId = Guid.Parse(findId);

        // Act
        var result = await repository.FindAsync(x => x.Id.Equals(refId), includeDetails: includeDetails);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Product>();

        if (includeDetails)
        {
            result.Category.Should().NotBeNull();
        }
    }

    #endregion
}