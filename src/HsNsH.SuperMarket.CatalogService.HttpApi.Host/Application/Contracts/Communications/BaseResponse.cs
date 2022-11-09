namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public abstract class BaseResponse
{
    public bool Success { get; }
    public string Message { get; }

    public int Code { get; }

    protected BaseResponse(bool success, string message, int code = 0)
    {
        Success = success;
        Message = message;
        Code = code;
    }
}