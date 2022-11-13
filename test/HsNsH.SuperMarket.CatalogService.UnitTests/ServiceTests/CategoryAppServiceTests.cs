using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Mapping;
using HsNsH.SuperMarket.CatalogService.Application.Services;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.ServiceTests;

public class CategoryAppServiceTests
{
    #region Setup

    private readonly Mock<ILogger<CategoryAppService>> _mockLogger = new();
    private readonly Mock<ICategoryRepository> _mockCategoryRepository = new();
    private readonly IMapper _mapper;

    public CategoryAppServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CatalogServiceEntityModelToDtoModelProfile>();
            cfg.AddProfile<CatalogServiceDtoModelToEntityModelProfile>();
        });
        _mapper = config.CreateMapper();
    }

    #endregion

    #region GetListAsync_Tests

    [Theory]
    [MemberData(nameof(GetAllExceptionsForThrowExceptionTests))]
    public async Task GetListAsync_WithThrowAllException_ThrowBusinessException(object throwException)
    {
        // Arrange
        switch (throwException)
        {
            case DomainException de:
                _mockCategoryRepository.Setup(service => service.GetListAsync(false))
                    .Throws(de);
                break;
            case BusinessException be:
                _mockCategoryRepository.Setup(service => service.GetListAsync(false))
                    .Throws(be);
                break;
            default:
                _mockCategoryRepository.Setup(service => service.GetListAsync(false))
                    .Throws((Exception)throwException);
                break;
        }

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.GetListAsync();

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();

        switch (throwException)
        {
            case DomainException de:
                result.And.Code.Should().Be(nameof(DomainException));
                result.And.Message.Should().Be(de.Message);
                result.And.InnerException.Should().Be(de.InnerException);
                break;
            case BusinessException be:
                result.And.Code.Should().Be(be.Code);
                result.And.Message.Should().Be(be.Message);
                result.And.InnerException.Should().Be(be.InnerException);
                break;
            default:
                // Application level exception
                result.And.Code.Should().Be("AppServiceException");
                result.And.Message.Should().NotBeNullOrWhiteSpace();
                // Source level exception
                result.And.InnerException.Should().NotBeNull();
                result.And.InnerException.Should().BeOfType<Exception>();
                result.And.InnerException?.Message.Should().Be(((Exception)throwException).Message);
                result.And.InnerException?.InnerException.Should().Be(((Exception)throwException).InnerException);
                break;
        }
    }

    [Fact]
    public async Task GetListAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryRepository.Setup(service => service.GetListAsync(false))
            .ReturnsAsync((List<Category>)null!);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.GetListAsync();

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();
        result.And.Code.Should().Be(nameof(DomainException));
    }

    [Fact]
    public async Task GetListAsync_WithExistingResponse_ReturnsExpectedObject()
    {
        // Arrange
        var expectedItems = new[] { CreateRandomCategory(), CreateRandomCategory(), CreateRandomCategory() };
        _mockCategoryRepository.Setup(service => service.GetListAsync(false))
            .ReturnsAsync(expectedItems);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var actualItems = await appService.GetListAsync();

        // Assert
        actualItems.Should().NotBeNull();
        actualItems.Should().BeOfType<List<CategoryDto>>();
        expectedItems.Should().BeEquivalentTo(actualItems, opt =>
            opt.ComparingByMembers<CategoryDto>());
    }

    #endregion

    #region GetByIdAsync_Tests

    [Fact]
    public async Task GetByIdAsync_WithThrowUnHandledException_ThrowBusinessException()
    {
        // Arrange
        var throwException = new Exception("Error message detail", new Exception("Inner Error Message Detail"));
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .Throws(throwException);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.GetByIdAsync(Guid.NewGuid());

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();

        // Application level exception
        result.And.Code.Should().Be("AppServiceException");
        result.And.Message.Should().NotBeNullOrWhiteSpace();
        // Source level exception
        result.And.InnerException.Should().NotBeNull();
        result.And.InnerException.Should().BeOfType<Exception>();
        result.And.InnerException?.Message.Should().Be(throwException.Message);
        result.And.InnerException?.InnerException.Should().Be(throwException.InnerException);
    }

    [Theory]
    [MemberData(nameof(GetHandledExceptionsForErrorResponseTests))]
    public async Task GetByIdAsync_WithThrowHandledException_ReturnsErrorObject(object throwException)
    {
        // Arrange
        switch (throwException)
        {
            case DomainException de:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws(de);
                break;
            default:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws((BusinessException)throwException);
                break;
        }

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidParameters_ReturnsErrorObject()
    {
        // Arrange
        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.GetByIdAsync(Guid.Empty);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnExistingResponse_ReturnsErrorObject()
    {
        // Arrange
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync((Category)null!);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingResponse_ReturnsExpectedObject()
    {
        // Arrange
        var expectedItem = CreateRandomCategory();
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(expectedItem);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(true);

        var value = result.Resource;
        value.Should().NotBeNull();
        value.Should().BeOfType<CategoryDto>();

        expectedItem.Should().BeEquivalentTo(value, opt => opt.ComparingByMembers<CategoryDto>());
    }

    #endregion

    #region CreateAsync_Tests

    [Fact]
    public async Task CreateAsync_WithThrowUnHandledException_ThrowBusinessException()
    {
        // Arrange
        var throwException = new Exception("Error message detail", new Exception("Inner Error Message Detail"));
        _mockCategoryRepository.Setup(c => c.InsertAsync(It.IsAny<Category>(), true))
            .Throws(throwException);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.CreateAsync(new CategoryCreateDto() { Name = Guid.NewGuid().ToString() });

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();

        // Application level exception
        result.And.Code.Should().Be("AppServiceException");
        result.And.Message.Should().NotBeNullOrWhiteSpace();
        // Source level exception
        result.And.InnerException.Should().NotBeNull();
        result.And.InnerException.Should().BeOfType<Exception>();
        result.And.InnerException?.Message.Should().Be(throwException.Message);
        result.And.InnerException?.InnerException.Should().Be(throwException.InnerException);
    }

    [Theory]
    [MemberData(nameof(GetHandledExceptionsForErrorResponseTests))]
    public async Task CreateAsync_WithThrowHandledException_ReturnsErrorObject(object throwException)
    {
        // Arrange
        switch (throwException)
        {
            case DomainException de:
                _mockCategoryRepository.Setup(c => c.InsertAsync(It.IsAny<Category>(), true))
                    .Throws(de);
                break;
            default:
                _mockCategoryRepository.Setup(c => c.InsertAsync(It.IsAny<Category>(), true))
                    .Throws((BusinessException)throwException);
                break;
        }

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.CreateAsync(new CategoryCreateDto() { Name = Guid.NewGuid().ToString() });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidParameters_ReturnsErrorObject()
    {
        // Arrange
        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.CreateAsync(null);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_WithValidParameters_ReturnsExpectedObject()
    {
        // Arrange
        var createdId = Guid.NewGuid();
        var itemToCreate = new CategoryCreateDto() { Name = Guid.NewGuid().ToString() };

        _mockCategoryRepository.Setup(repo => repo.InsertAsync(It.IsAny<Category>(), true))
            .ReturnsAsync(new Category() { Id = createdId, Name = itemToCreate.Name });

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.CreateAsync(itemToCreate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(true);

        var createdItem = result.Resource;
        createdItem.Id.Should().Be(createdId);
        createdItem.Should().BeEquivalentTo(itemToCreate, opt => opt.ComparingByMembers<CategoryDto>());
    }

    #endregion

    #region UpdateAsync_Tests

    [Fact]
    public async Task UpdateAsync_WithThrowUnHandledException_ThrowBusinessException()
    {
        // Arrange
        var throwException = new Exception("Error message detail", new Exception("Inner Error Message Detail"));
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .Throws(throwException);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.UpdateAsync(Guid.NewGuid(), new CategoryUpdateDto() { Name = Guid.NewGuid().ToString() });

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();

        // Application level exception
        result.And.Code.Should().Be("AppServiceException");
        result.And.Message.Should().NotBeNullOrWhiteSpace();
        // Source level exception
        result.And.InnerException.Should().NotBeNull();
        result.And.InnerException.Should().BeOfType<Exception>();
        result.And.InnerException?.Message.Should().Be(throwException.Message);
        result.And.InnerException?.InnerException.Should().Be(throwException.InnerException);
    }

    [Theory]
    [MemberData(nameof(GetHandledExceptionsForErrorResponseTests))]
    public async Task UpdateAsync_WithThrowHandledException_ReturnsErrorObject(object throwException)
    {
        // Arrange
        switch (throwException)
        {
            case DomainException de:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws(de);
                break;
            default:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws((BusinessException)throwException);
                break;
        }

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.UpdateAsync(Guid.NewGuid(), new CategoryUpdateDto() { Name = Guid.NewGuid().ToString() });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetInvalidInputForUpdate))]
    public async Task UpdateAsync_WithInvalidParameters_ReturnsErrorObject(Guid itemId, CategoryUpdateDto itemToUpdate)
    {
        // Arrange
        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.UpdateAsync(itemId, itemToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAsync_WithUnExistingResponse_ReturnsErrorObject()
    {
        // Arrange
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync((Category)null!);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.UpdateAsync(Guid.NewGuid(), new CategoryUpdateDto() { Name = Guid.NewGuid().ToString() });

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingResponse_ReturnsExpectedObject()
    {
        // Arrange
        var updateId = Guid.NewGuid();
        var itemToUpdate = new CategoryUpdateDto() { Name = Guid.NewGuid().ToString() };

        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(CreateRandomCategory());

        _mockCategoryRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Category>(), true))
            .ReturnsAsync(new Category() { Id = updateId, Name = itemToUpdate.Name });

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.UpdateAsync(updateId, itemToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(true);

        var updatedItem = result.Resource;
        updatedItem.Id.Should().NotBeEmpty();
        updatedItem.Should().BeEquivalentTo(itemToUpdate, opt => opt.ComparingByMembers<CategoryDto>());
    }

    #endregion

    #region DeleteAsync_Tests

    [Fact]
    public async Task DeleteAsync_WithThrowUnHandledException_ThrowBusinessException()
    {
        // Arrange
        var throwException = new Exception("Error message detail", new Exception("Inner Error Message Detail"));
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .Throws(throwException);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        Func<Task> act = async () => await appService.DeleteAsync(Guid.NewGuid());

        // Assert
        var result = await act.Should().ThrowAsync<BusinessException>();

        // Application level exception
        result.And.Code.Should().Be("AppServiceException");
        result.And.Message.Should().NotBeNullOrWhiteSpace();
        // Source level exception
        result.And.InnerException.Should().NotBeNull();
        result.And.InnerException.Should().BeOfType<Exception>();
        result.And.InnerException?.Message.Should().Be(throwException.Message);
        result.And.InnerException?.InnerException.Should().Be(throwException.InnerException);
    }

    [Theory]
    [MemberData(nameof(GetHandledExceptionsForErrorResponseTests))]
    public async Task DeleteAsync_WithThrowHandledException_ReturnsErrorObject(object throwException)
    {
        // Arrange
        switch (throwException)
        {
            case DomainException de:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws(de);
                break;
            default:
                _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
                    .Throws((BusinessException)throwException);
                break;
        }

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BaseResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidParameters_ReturnsErrorObject()
    {
        // Arrange
        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.DeleteAsync(Guid.Empty);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BaseResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_WithUnExistingResponse_ReturnsErrorObject()
    {
        // Arrange
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync((Category)null!);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BaseResponse>();
        result.Success.Should().Be(false);
        result.Messages.Should().NotBeEmpty();
        result.Code.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingResponse_ReturnsExpectedObject()
    {
        // Arrange
        var expectedItem = CreateRandomCategory();
        _mockCategoryRepository.Setup(c => c.FindAsync(It.IsAny<Expression<Func<Category, bool>>>(), true))
            .ReturnsAsync(expectedItem);

        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BaseResponse>();
        result.Success.Should().Be(true);
    }

    #endregion

    #region Helper Functions

    private static Category CreateRandomCategory()
    {
        return new Category { Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString() };
    }

    public static IEnumerable<object?[]> GetInvalidInputForUpdate()
    {
        yield return new object?[] { Guid.Empty, null };
        yield return new object?[] { Guid.Empty, new CategoryUpdateDto() { Name = "Test" } };
        yield return new object?[] { Guid.NewGuid(), null };
    }

    public static IEnumerable<object?[]> GetAllExceptionsForThrowExceptionTests()
    {
        yield return new object?[] { new Exception("Error message detail", new Exception("Inner Error Message Detail")) };
        yield return new object?[] { new DomainException("Domain exception detail", new Exception("Dead lock")) };
        yield return new object?[] { new BusinessException("App:12345", "Invalid parameters", new DomainException("id is invalid")) };
        yield return new object?[] { new BusinessException("App:98765", "Data fetch error", new Exception("Dead lock")) };
    }

    public static IEnumerable<object?[]> GetHandledExceptionsForErrorResponseTests()
    {
        yield return new object?[] { new DomainException("Domain exception detail", new Exception("Dead lock")) };
        yield return new object?[] { new BusinessException("App:12345", "Invalid parameters", new DomainException("id is invalid")) };
        yield return new object?[] { new BusinessException("App:98765", "Data fetch error", new Exception("Dead lock")) };
    }

    #endregion
}