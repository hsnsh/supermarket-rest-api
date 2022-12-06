using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers.v1;

[Route("/api/v1/[controller]")]
public class DataController : CatalogServiceController
{
    private readonly IDataAppService _dataAppService;

    public DataController(IDataAppService dataAppService)
    {
        _dataAppService = dataAppService;
    }

    [HttpGet("GetCategories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _dataAppService.GetListAsync());
    }

    [HttpGet("GetCategoriesWithNagigations")]
    public async Task<IActionResult> GetCategoriesWithNagigations()
    {
        return Ok(await _dataAppService.GetListWithNavigationsAsync());
    }
}