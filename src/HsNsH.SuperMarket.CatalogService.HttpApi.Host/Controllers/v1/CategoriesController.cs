using System.Net;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers.v1;

[Route("/api/v1/[controller]")]
public class CategoriesController : CatalogServiceController
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryAppService _categoryAppService;

    public CategoriesController(ILogger<CategoriesController> logger, ICategoryAppService categoryAppService)
    {
        _logger = logger;
        _categoryAppService = categoryAppService;
    }

    // GET /categories
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var items = await _categoryAppService.GetListAsync();
        if (items == null)
        {
            throw new BusinessException("CatalogService:000001", "An error occurred while retrieving data");
        }

        _logger.LogInformation("{S}: Retrieved {Count} categories", DateTime.UtcNow.ToString("HH:mm:ss"), items.Count);

        return Ok(items);
    }

    // GET /categories/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoryByIdAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) return BadRequest(new ErrorDto("Invalid id", ((int)HttpStatusCode.BadRequest).ToString()));

        var response = await _categoryAppService.GetByIdAsync(id);
        if (response == null)
        {
            throw new BusinessException("CatalogService:000001", "An error occurred while retrieving data");
        }

        if (!response.Success)
        {
            return BadRequest(new ErrorDto(response.Message, response.Code.ToString()));
        }

        return Ok(response.Resource);
    }

    // POST /categories
    [HttpPost]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryCreateDto input)
    {
        if (input == null) return BadRequest(new ErrorDto("Input parameter can not be null", ((int)HttpStatusCode.BadRequest).ToString()));

        var response = await _categoryAppService.CreateAsync(input);
        if (response == null)
        {
            throw new BusinessException("CatalogService:000001", "An error occurred while retrieving data");
        }

        if (!response.Success)
        {
            return BadRequest(new ErrorDto(response.Message, response.Code.ToString()));
        }

        return CreatedAtAction("GetCategoryById", new { id = response.Resource.Id }, response.Resource);
    }

    // PUT /categories/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateCategoryAsync(Guid id, [FromBody] CategoryUpdateDto input)
    {
        if (id.Equals(Guid.Empty)) return BadRequest(new ErrorDto("Invalid id", ((int)HttpStatusCode.BadRequest).ToString()));
        if (input == null) return BadRequest(new ErrorDto("Input parameter can not be null", ((int)HttpStatusCode.BadRequest).ToString()));

        var response = await _categoryAppService.UpdateAsync(id, input);
        if (response == null)
        {
            throw new BusinessException("CatalogService:000001", "An error occurred while retrieving data");
        }

        if (!response.Success)
        {
            return BadRequest(new ErrorDto(response.Message, response.Code.ToString()));
        }

        return Ok(response.Resource);
    }

    // DELETE /categories/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteCategoryAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) return BadRequest(new ErrorDto("Invalid id", ((int)HttpStatusCode.BadRequest).ToString()));

        var response = await _categoryAppService.DeleteAsync(id);
        if (response == null)
        {
            throw new BusinessException("CatalogService:000001", "An error occurred while retrieving data");
        }

        if (!response.Success)
        {
            return BadRequest(new ErrorDto(response.Message, response.Code.ToString()));
        }

        return NoContent();
    }
}