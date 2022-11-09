using System.Net;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
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
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var items = await _categoryAppService.GetListAsync();
        if (items == null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto($"An error occurred while retrieving data"));
        }

        _logger.LogInformation("{S}: Retrieved {Count} categories", DateTime.UtcNow.ToString("HH:mm:ss"), items.Count);

        return Ok(items);
    }

    // GET /categories/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType(typeof(ErrorDto), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCategoryByIdAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) return BadRequest(new ErrorDto("Invalid id"));

        var response = await _categoryAppService.GetByIdAsync(id);
        if (response == null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError
                , new ErrorDto($"An error occurred while retrieving data"));
        }

        if (response.HasError)
        {
            return response.IsInternalError
                ? StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto(response.Message))
                : BadRequest(new ErrorDto(response.Message));
        }

        return Ok(response.Resource);
    }
}