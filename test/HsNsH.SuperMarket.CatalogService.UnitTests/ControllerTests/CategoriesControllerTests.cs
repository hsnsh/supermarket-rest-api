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
        var expectedItems = new[] { CreateRandomCategoryDto(), CreateRandomCategoryDto(), CreateRandomCategoryDto() };
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
        var expectedItem = CreateRandomCategoryDto();
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

    [Fact]
    public async Task CreateCategoryAsync_WithInvalidParameters_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.CreateCategoryAsync(null);

        // Assert
        VerifyBadRequestObjectResult(result, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryAppService.Setup(repo => repo.CreateAsync(It.IsAny<CategoryCreateDto>()))
            .ReturnsAsync((CategoryDtoResponse)null!);

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        Func<Task> act = async () => await controller.CreateCategoryAsync(new CategoryCreateDto());

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task CreateCategoryAsync_WithErrorCategoryResponse_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var expectedStatus = (int)HttpStatusCode.Conflict;
        _mockCategoryAppService.Setup(repo => repo.CreateAsync(It.IsAny<CategoryCreateDto>()))
            .ReturnsAsync(new CategoryDtoResponse("Conflict error", expectedStatus));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.CreateCategoryAsync(new CategoryCreateDto());

        // Assert
        VerifyBadRequestObjectResult(result, expectedStatus);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithCategoryToCreate_ReturnsCreatedCategory()
    {
        // Arrange
        var itemToCreate = new CategoryCreateDto() { Name = Guid.NewGuid().ToString() };

        _mockCategoryAppService.Setup(repo => repo.CreateAsync(It.IsAny<CategoryCreateDto>()))
            .ReturnsAsync(new CategoryDtoResponse(new CategoryDto() { Id = Guid.NewGuid(), Name = itemToCreate.Name }));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.CreateCategoryAsync(itemToCreate);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        var createdItem = (result as CreatedAtActionResult)?.Value as CategoryDto;
        itemToCreate.Should().BeEquivalentTo(createdItem, opt => opt.ComparingByMembers<CategoryDto>().ExcludingMissingMembers());
        createdItem?.Id.Should().NotBeEmpty();
        // time-check => createdItem?.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1))
    }

    [Theory]
    [MemberData(nameof(GetInvalidInputForUpdate))]
    public async Task UpdateCategoryAsync_WithInvalidParameters_ReturnsBadRequestWithErrorObject(Guid itemId, CategoryUpdateDto itemToUpdate)
    {
        // Arrange
        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.UpdateCategoryAsync(itemId, itemToUpdate);

        // Assert
        VerifyBadRequestObjectResult(result, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryAppService.Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryUpdateDto>()))
            .ReturnsAsync((CategoryDtoResponse)null!);

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        Func<Task> act = async () => await controller.UpdateCategoryAsync(Guid.NewGuid(), new CategoryUpdateDto());

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithErrorCategoryResponse_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var expectedStatus = (int)HttpStatusCode.Conflict;
        _mockCategoryAppService.Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryUpdateDto>()))
            .ReturnsAsync(new CategoryDtoResponse("Conflict error", expectedStatus));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.UpdateCategoryAsync(Guid.NewGuid(), new CategoryUpdateDto());

        // Assert
        VerifyBadRequestObjectResult(result, expectedStatus);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithCategoryToUpdate_ReturnsUpdatedCategory()
    {
        // Arrange
        var updateId = Guid.NewGuid();
        var itemToUpdate = new CategoryUpdateDto() { Name = Guid.NewGuid().ToString() };

        _mockCategoryAppService.Setup(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryUpdateDto>()))
            .ReturnsAsync(new CategoryDtoResponse(new CategoryDto() { Id = updateId, Name = itemToUpdate.Name }));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.UpdateCategoryAsync(updateId, itemToUpdate);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var updatedItem = ((OkObjectResult)result!)?.Value;
        updatedItem.Should().NotBeNull();
        updatedItem.Should().BeOfType<CategoryDto>();
        ((CategoryDto)updatedItem!).Id.Should().NotBeEmpty();
        itemToUpdate.Should().BeEquivalentTo(updatedItem, opt => opt.ComparingByMembers<CategoryDto>().ExcludingMissingMembers());
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithInvalidParameters_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.DeleteCategoryAsync(Guid.Empty);

        // Assert
        VerifyBadRequestObjectResult(result, (int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithNullResponse_ThrowBusinessException()
    {
        // Arrange
        _mockCategoryAppService.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync((BaseResponse)null!);

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        Func<Task> act = async () => await controller.DeleteCategoryAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithUnExistingCategoryResponse_ReturnsBadRequestWithErrorObject()
    {
        // Arrange
        var expectedStatus = (int)HttpStatusCode.NotFound;
        _mockCategoryAppService.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new CategoryDtoResponse("Not found", expectedStatus));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.DeleteCategoryAsync(Guid.NewGuid());

        // Assert
        VerifyBadRequestObjectResult(result, expectedStatus);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithExistingCategoryResponse_ReturnsNoContent()
    {
        // Arrange
        _mockCategoryAppService.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BaseResponse(true, string.Empty));

        var controller = new CategoriesController(_mockLogger.Object, _mockCategoryAppService.Object);

        // Act
        var result = await controller.DeleteCategoryAsync(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private static CategoryDto CreateRandomCategoryDto()
    {
        return new CategoryDto { Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString() };
    }

    public static IEnumerable<object?[]> GetInvalidInputForUpdate()
    {
        yield return new object?[] { Guid.Empty, null };
        yield return new object?[] { Guid.Empty, new CategoryUpdateDto() { Name = "Test" } };
        yield return new object?[] { Guid.NewGuid(), null };
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