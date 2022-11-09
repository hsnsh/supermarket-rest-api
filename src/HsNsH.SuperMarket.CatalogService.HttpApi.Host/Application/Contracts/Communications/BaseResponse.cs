namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public abstract class BaseResponse
{
    public string Message { get; }
    public bool HasError { get; }
    public bool IsInternalError { get; }

    protected BaseResponse(string message, bool hasError, bool isInternalError = false)
    {
        Message = message;
        HasError = hasError;
        IsInternalError = isInternalError;
    }
}