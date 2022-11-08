using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }
}