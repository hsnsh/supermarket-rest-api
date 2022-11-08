using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ILogger<CategoryController> logger)
    {
        _logger = logger;
    }
}