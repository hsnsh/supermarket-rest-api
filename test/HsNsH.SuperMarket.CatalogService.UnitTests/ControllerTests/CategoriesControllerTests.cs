using System.Net;
using FluentAssertions;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Controllers.v1;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HsNsH.SuperMarket.CatalogService.UnitTests.ControllerTests;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryAppService> _mockCategoryAppService = new();
    private readonly Mock<ILogger<CategoriesController>> _mockLogger = new();

    /// <summary>
    /// Test Function Implementation : UnitOfWork_StateUnderTest_ExpectedBehavior
    /// </summary>
    [Fact]
    public async Task GetCategoriesAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryAppService.Setup(service => service.GetListAsync())
            .ReturnsAsync((List<CategoryDto>)null!);

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        Func<Task> act = async () => await controller.GetCategoriesAsync();

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task GetCategoriesAsync_WithExistingCategories_ReturnsAllCategories()
    {
        // Arrange
        var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        _mockCategoryAppService.Setup(service => service.GetListAsync())
            .ReturnsAsync(expectedItems.ToList());

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var actualItems = await controller.GetCategoriesAsync();

        // Assert
        actualItems.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)actualItems).Value;
        value.Should().NotBeNull();
        value.Should().BeEquivalentTo(expectedItems, opt => opt.ComparingByMembers<CategoryDto>());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidParameters_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.GetCategoryByIdAsync(Guid.Empty);

        // Assert
        VerifyBadRequestObjectResult(result, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryAppService.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((CategoryDtoResponse)null!);

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        Func<Task> act = async () => await controller.GetCategoryByIdAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithUnExistingCategoryResponse_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var expectedStatus = (int)HttpStatusCode.NotFound;
        _mockCategoryAppService.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new CategoryDtoResponse("Not found", expectedStatus));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.GetCategoryByIdAsync(Guid.NewGuid());

        // Assert
        VerifyBadRequestObjectResult(result, expectedStatus);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithExistingCategoryResponse_ReturnsExpectedCategory()
    {
        // Arrange
        var expectedItem = CreateRandomItem();
        _mockCategoryAppService.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new CategoryDtoResponse(expectedItem));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.GetCategoryByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)result!)?.Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<CategoryDto>();

        value.Should().BeEquivalentTo(expectedItem, opt => opt.ComparingByMembers<CategoryDto>());
    }

    private static CategoryDto CreateRandomItem()
    {
        return new CategoryDto { Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString() };
    }

    private static void VerifyBadRequestObjectResult(object result, int? code = null)
    {
        result.Should().BeOfType<BadRequestObjectResult>();

        var value = ((BadRequestObjectResult)result).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<ErrorDto>();

        var messages = ((ErrorDto)value!)?.Messages;
        messages.Should().NotBeNull();
        messages.Should().NotHaveCount(0);

        if (!code.HasValue) return;
        var statusCode = ((ErrorDto)value!)?.Code;
        statusCode.Should().NotBeNullOrWhiteSpace();
        statusCode.Should().Be(code.Value.ToString());
    }
}