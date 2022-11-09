using Microsoft.AspNetCore.Mvc;

namespace HsNsH.SuperMarket.CatalogService.Controllers;

[Produces("application/json")]
[ApiController]
public abstract class CatalogServiceController : ControllerBase
{
    protected CatalogServiceController()
    {
    }
}