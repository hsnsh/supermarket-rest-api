using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Enums;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using HsNsH.SuperMarket.CatalogService.Persistence.Repositories;
using HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests.TestBase;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.DomainTests;

public class GenericRepositoryTests : BaseRepositoryTests
{
    #region GetCountAsync_Tests

    [Fact]
    public async Task GetCountAsync_WithAllItems_ReturnsAllItemsCount()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var expectedCount = await context.Products.LongCountAsync();

        // Act
        var result = await repository.GetCountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    #endregion

    #region Helper Functions

    private static Product CreateRandomProduct(Guid? id = null, Guid? categoryId = null)
    {
        var product = new Product();
        if (id.HasValue) product = new Product(id.Value);
        product.Name = Guid.NewGuid().ToString();
        product.QuantityInPackage = 1;
        product.UnitOfMeasurement = EUnitOfMeasurement.Unity;
        if (categoryId.HasValue) product.CategoryId = categoryId.Value;

        return product;
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
        result.And.Message.Should().NotBeNullOrWhiteSpace();
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

    #region GetAsync_Tests

    [Fact]
    public async Task GetAsync_WithUnExistingItem_ThrowDomainException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var refId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await repository.GetAsync(x => x.Id.Equals(refId), includeDetails: false);

        // Assert
        var result = await act.Should().ThrowAsync<DomainException>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetAsync_WithDuplicateItems_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.GetAsync(x => x.Name.Contains("Product"), includeDetails: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("b61cf65c-edfc-4a23-90a8-112fd957fab5", true)]
    [InlineData("b61cf65c-edfc-4a23-90a8-112fd957fab5", false)]
    public async Task GetAsync_WithExistingItem_ReturnsExpectedItem(string findId, bool includeDetails)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var refId = Guid.Parse(findId);

        // Act
        var result = await repository.GetAsync(x => x.Id.Equals(refId), includeDetails: includeDetails);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Product>();

        if (includeDetails)
        {
            result.Category.Should().NotBeNull();
        }
    }

    #endregion

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

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData($"{nameof(Product.Name)} desc", false)]
    [InlineData($"{nameof(Product.Name)} asc", false)]
    public async Task GetListAsync_WithFilter_ReturnsFilteredItems(string sorting, bool includeDetails)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var expectedCount = await context.Products.CountAsync(x => x.Name.Contains("Populate"));

        // Act
        var actualItems = await repository.GetListAsync(x => x.Name.Contains("Populate"), sorting: sorting, includeDetails: includeDetails);

        // Assert
        var items = actualItems as Product[] ?? actualItems.ToArray();
        items.Should().BeOfType<Product[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);

        if (!string.IsNullOrWhiteSpace(sorting))
        {
            var expectedAscItemName = await context.Products.Where(x => x.Name.Contains("Populate")).OrderBy(x => x.Name).Select(x => x.Name).FirstAsync();
            var expectedDescItemName = await context.Products.Where(x => x.Name.Contains("Populate")).OrderByDescending(x => x.Name).Select(x => x.Name).FirstAsync();
            items[0].Name.Should().Be(sorting.Split(' ')[1].Equals("desc") ? expectedDescItemName : expectedAscItemName);
        }

        if (includeDetails)
        {
            items.Any(x => x.Category != null).Should().Be(true);
        }
    }

    #endregion

    #region GetPageListAsync_Tests

    [Theory]
    [InlineData(-1, 4, 4)]
    [InlineData(0, -4, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(0, 4, 4)]
    [InlineData(1, -4, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(1, 4, 4)]
    [InlineData(1000000, 4, 0)] //page not found
    public async Task GetPageListAsync_WithMaxResultAndSkipCount_ReturnsExpectedPageItems(int pageIndex, int pageSize, int expectedCount)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var skipCount = pageIndex * pageSize;

        // Act
        var actualItems = await repository.GetPageListAsync(skipCount: skipCount, maxResultCount: pageSize, sorting: null, includeDetails: false);

        // Assert
        var items = actualItems as Product[] ?? actualItems.ToArray();
        items.Should().BeOfType<Product[]>();
        items.Should().NotBeNull();
        items.Should().HaveCountLessThanOrEqualTo(expectedCount);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData($"{nameof(Product.Name)} desc", false)]
    [InlineData($"{nameof(Product.Name)} asc", false)]
    public async Task GetPageListAsync_WithSorting_ReturnsSortedItems(string sorting, bool includeDetails)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var expectedCount = await context.Products.Skip(4).Take(4).CountAsync();

        // Act
        var actualItems = await repository.GetPageListAsync(skipCount: 4, maxResultCount: 4, sorting: sorting, includeDetails: includeDetails);

        // Assert
        var items = actualItems as Product[] ?? actualItems.ToArray();
        items.Should().BeOfType<Product[]>();
        items.Should().NotBeNull();
        items.Should().HaveCount(expectedCount);

        if (!string.IsNullOrWhiteSpace(sorting))
        {
            var expectedAscItemName = await context.Products.OrderBy(x => x.Name).Skip(4).Take(4).Select(x => x.Name).FirstAsync();
            var expectedDescItemName = await context.Products.OrderByDescending(x => x.Name).Skip(4).Take(4).Select(x => x.Name).FirstAsync();
            items[0].Name.Should().Be(sorting.Split(' ')[1].Equals("desc") ? expectedDescItemName : expectedAscItemName);
        }

        if (includeDetails)
        {
            items.Any(x => x.Category != null).Should().Be(true);
        }
    }

    #endregion

    #region InsertAsync_Tests

    [Fact]
    public async Task InsertAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.InsertAsync(null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task InsertAsync_WithObjectToCreate_ReturnsCreatedObject(bool includeId, bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var createdId = Guid.NewGuid();
        var itemToCreate = CreateRandomProduct(id: includeId ? createdId : null, categoryId: Guid.NewGuid());

        // Act
        var createdItem = await repository.InsertAsync(itemToCreate, autoSave);

        // Assert
        createdItem.Should().NotBeNull();
        createdItem.Should().BeOfType<Product>();
        createdItem.Id.Should().NotBeEmpty();
        itemToCreate.Should().BeEquivalentTo(createdItem, opt => opt.ComparingByValue<Product>());
        if (includeId)
        {
            createdItem.Id.Should().Be(createdId);
        }
        else
        {
            createdId = itemToCreate.Id;
        }

        var entity = await context.Products.FirstOrDefaultAsync(x => x.Id.Equals(createdId));
        if (autoSave)
        {
            entity.Should().NotBeNull();
        }
        else
        {
            entity.Should().BeNull();
        }
    }

    #endregion

    #region InsertManyAsync_Tests

    [Fact]
    public async Task InsertManyAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.InsertManyAsync(null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InsertManyAsync_WithObjectListToCreate_ReturnsCreatedListObject(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var itemsToCreate = new[] { CreateRandomProduct(id: Guid.NewGuid()), CreateRandomProduct(id: Guid.NewGuid()) };

        // Act
        await repository.InsertManyAsync(itemsToCreate, autoSave);

        // Assert
        var ids = itemsToCreate.Select(x => x.Id).ToList();
        var entity = await context.Products.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        if (autoSave)
        {
            entity.Should().NotBeNull();
            entity.Should().BeOfType<Product[]>();
            entity.Should().HaveCount(itemsToCreate.Length);
            itemsToCreate.Should().BeEquivalentTo(entity, opt => opt.ComparingByValue<Product>());
        }
        else
        {
            entity.Should().BeEmpty();
        }
    }

    #endregion

    #region UpdateAsync_Tests

    [Fact]
    public async Task UpdateAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.UpdateAsync(null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateAsync_WithUnExistingItem_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var updateItem = CreateRandomProduct(id: Guid.NewGuid(), categoryId: Guid.NewGuid());

        // Act
        Func<Task> act = async () => await repository.UpdateAsync(updateItem, true);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateAsync_WithObjectToUpdate_ReturnsUpdatedObject(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var updateId = Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5");
        var itemToUpdate = await context.Products.FindAsync(updateId);
        itemToUpdate!.Name = Guid.NewGuid().ToString();
        itemToUpdate.CategoryId = Guid.Empty;

        // Act
        var updatedItem = await repository.UpdateAsync(itemToUpdate, autoSave);

        // Assert
        updatedItem.Should().NotBeNull();
        updatedItem.Should().BeOfType<Product>();
        updatedItem.Id.Should().NotBeEmpty();
        itemToUpdate.Should().BeEquivalentTo(updatedItem, opt => opt.ComparingByValue<Product>());

        var entity = await context.Products.FirstAsync(x => x.Id.Equals(updateId));
        var entry = context.Entry(entity);
        if (autoSave)
        {
            entry.State.Should().Be(EntityState.Unchanged);
            entity.Should().BeEquivalentTo(updatedItem, opt => opt.ComparingByValue<Product>());
        }
        else
        {
            entry.State.Should().Be(EntityState.Modified);
        }
    }

    #endregion

    #region UpdateManyAsync_Tests

    [Fact]
    public async Task UpdateManyAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.UpdateManyAsync(null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateManyAsync_WithUnExistingItem_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var itemsToUpdates = new[] { CreateRandomProduct(id: Guid.NewGuid()), CreateRandomProduct(id: Guid.NewGuid()) };

        // Act
        Func<Task> act = async () => await repository.UpdateManyAsync(itemsToUpdates, true);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateManyAsync_WithObjectListToUpdate_ReturnsVoid(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var updateId = Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5");
        var itemToUpdate = await context.Products.FindAsync(updateId);
        itemToUpdate!.Name = Guid.NewGuid().ToString();
        itemToUpdate.CategoryId = Guid.Empty;

        // Act
        await repository.UpdateManyAsync(new[] { itemToUpdate }, autoSave);

        // Assert
        var entity = await context.Products.FirstAsync(x => x.Id.Equals(updateId));
        var entry = context.Entry(entity);
        if (autoSave)
        {
            entry.State.Should().Be(EntityState.Unchanged);
            entity.Should().BeEquivalentTo(itemToUpdate, opt => opt.ComparingByValue<Product>());
        }
        else
        {
            entry.State.Should().Be(EntityState.Modified);
        }
    }

    #endregion

    #region DeleteAsync_Tests

    [Fact]
    public async Task DeleteAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.DeleteAsync(entity: null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task DeleteAsync_WithUnExistingItem_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var updateItem = CreateRandomProduct(id: Guid.NewGuid(), categoryId: Guid.NewGuid());

        // Act
        Func<Task> act = async () => await repository.DeleteAsync(updateItem, true);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteAsync_WithObjectToDelete_ReturnsVoid(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var deleteId = Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5");
        var itemToDelete = await context.Products.FindAsync(deleteId);

        // Act
        await repository.DeleteAsync(itemToDelete!, autoSave);

        // Assert
        var entity = await context.Products.FirstOrDefaultAsync(x => x.Id.Equals(deleteId));
        if (autoSave)
        {
            entity.Should().BeNull();
        }
        else
        {
            entity.Should().NotBeNull();
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteAsync_WithPredicate_ReturnsVoid(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var deleteId = Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5");

        // Act
        await repository.DeleteAsync(x => x.Id.Equals(deleteId), autoSave);

        // Assert
        var entity = await context.Products.FirstOrDefaultAsync(x => x.Id.Equals(deleteId));
        if (autoSave)
        {
            entity.Should().BeNull();
        }
        else
        {
            entity.Should().NotBeNull();
        }
    }

    #endregion

    #region DeleteManyAsync_Tests

    [Fact]
    public async Task DeleteManyAsync_WithNullObject_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        // Act
        Func<Task> act = async () => await repository.DeleteManyAsync(null!, autoSave: false);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task DeleteManyAsync_WithUnExistingItem_ThrowException()
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);
        var itemsToDelete = new[] { CreateRandomProduct(id: Guid.NewGuid()), CreateRandomProduct(id: Guid.NewGuid()) };

        // Act
        Func<Task> act = async () => await repository.DeleteManyAsync(itemsToDelete, true);

        // Assert
        var result = await act.Should().ThrowAsync<Exception>();
        result.And.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteManyAsync_WithObjectToDelete_ReturnsVoid(bool autoSave)
    {
        // Arrange
        var context = await CreateDefaultContextAsync();
        var repository = new GenericRepository<CatalogServiceTestDbContext, Product>(context);

        var deleteId = Guid.Parse("b61cf65c-edfc-4a23-90a8-112fd957fab5");
        var itemToDelete = await context.Products.FirstAsync(x => x.Id == deleteId);

        // Act
        await repository.DeleteManyAsync(new[] { itemToDelete }, autoSave);

        // Assert
        var entity = await context.Products.FirstOrDefaultAsync(x => x.Id.Equals(deleteId));
        if (autoSave)
        {
            entity.Should().BeNull();
        }
        else
        {
            entity.Should().NotBeNull();
        }
    }

    #endregion
}