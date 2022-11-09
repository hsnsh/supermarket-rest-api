using System.Text.Json;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HsNsH.SuperMarket.CatalogService.Filters;

/// <summary>
/// Used for error handling.  
/// </summary>
public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<CustomExceptionFilterAttribute> _logger;
    private readonly IWebHostEnvironment _env;

    public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);

        _logger.LogError(context.Exception, "InternalServerError -> {ErrorMessage}", context.Exception.Message);

        var code = StatusCodes.Status500InternalServerError.ToString();
        List<string> messages = new List<string>();

        // Some logic to handle specific exceptions
        if (context.Exception is BusinessException be)
        {
            if (!string.IsNullOrWhiteSpace(be.Code)) code = be.Code;
            if (!string.IsNullOrWhiteSpace(be.Message)) messages.Add(be.Message);
            if (be.InnerException != null && _env.IsDevelopment())
            {
                messages = PopulateInnerExceptionErrors(messages, be.InnerException);
            }
        }
        else if (context.Exception is DomainException de)
        {
            if (!string.IsNullOrWhiteSpace(de.Message)) messages.Add(de.Message);
            if (de.InnerException != null && _env.IsDevelopment())
            {
                messages = PopulateInnerExceptionErrors(messages, de.InnerException);
            }
        }
        else
        {
            messages.Add("Internal Server Error");
            if (_env.IsDevelopment())
            {
                messages = PopulateInnerExceptionErrors(messages, context.Exception);
            }
        }

        // Returning response
        context.Result = new ContentResult { Content = JsonSerializer.Serialize(new ErrorDto(messages, code)), StatusCode = StatusCodes.Status500InternalServerError, ContentType = "application/json" };
    }

    private static List<string> PopulateInnerExceptionErrors(List<string> source, Exception innerException)
    {
        var currentExc = innerException;
        while (currentExc != null)
        {
            source.Add(currentExc.Message);
            currentExc = currentExc.InnerException;
        }

        return source;
    }
}