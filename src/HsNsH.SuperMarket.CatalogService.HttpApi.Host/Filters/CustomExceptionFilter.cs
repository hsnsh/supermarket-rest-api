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
        var messages = new List<string>();

        switch (context.Exception)
        {
            // Some logic to handle specific exceptions
            case BusinessException be:
            {
                if (!string.IsNullOrWhiteSpace(be.Code)) code = be.Code;
                if (!string.IsNullOrWhiteSpace(be.Message)) messages.Add(be.Message);
                if (be.InnerException != null && _env.IsDevelopment())
                {
                    messages = PopulateInnerExceptionErrors(messages, be.InnerException);
                }

                break;
            }
            case DomainException de:
            {
                code = nameof(DomainException);
                if (!string.IsNullOrWhiteSpace(de.Message)) messages.Add(de.Message);
                if (de.InnerException != null && _env.IsDevelopment())
                {
                    messages = PopulateInnerExceptionErrors(messages, de.InnerException);
                }

                break;
            }
            default:
            {
                messages.Add("Internal Server Error");
                if (_env.IsDevelopment())
                {
                    messages = PopulateInnerExceptionErrors(messages, context.Exception);
                }

                break;
            }
        }
        
        
        
        //
        // return ex switch
        // {
        //     BusinessException be => throw be,
        //     DomainException de => throw new BusinessException(nameof(DomainException), de.Message, de.InnerException),
        //     _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
        // };
        
        
        
        

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