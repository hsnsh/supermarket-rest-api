using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers.v1;

[Route("/api/v1/[controller]")]
public class ProductsController : CatalogServiceController
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductAppService _productAppService;

    public ProductsController(ILogger<ProductsController> logger, IProductAppService productAppService)
    {
        _logger = logger;
        _productAppService = productAppService;
    }
}