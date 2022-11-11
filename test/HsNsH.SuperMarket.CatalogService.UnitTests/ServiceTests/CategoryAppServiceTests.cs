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
    public async Task GetListAsync_WithExistingCategories_ReturnsAllCategories()
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
    public async Task GetByIdAsync_WithThrowHandledException_ReturnsObjectWithErrorDetails(object throwException)
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
        result.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidParameters_ReturnsObjectWithErrorDetails()
    {
        // Arrange
        var appService = new CategoryAppService(_mockLogger.Object, _mockCategoryRepository.Object, _mapper);

        // Act
        var result = await appService.GetByIdAsync(Guid.Empty);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CategoryDtoResponse>();
        result.Success.Should().Be(false);
        result.Message.Should().NotBeNullOrWhiteSpace();
        result.Code.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnExistingCategoryResponse_ReturnsObjectWithErrorDetails()
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
        result.Message.Should().NotBeNullOrWhiteSpace();
        result.Code.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingCategoryResponse_ReturnsExpectedCategory()
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

    private static Category CreateRandomCategory()
    {
        return new Category { Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString() };
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
}