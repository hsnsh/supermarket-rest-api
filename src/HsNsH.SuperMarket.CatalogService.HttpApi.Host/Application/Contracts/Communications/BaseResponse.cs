namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public class BaseResponse : IBaseResponse
{
    public bool Success { get; }
    public string Message { get; }

    public int Code { get; }

    public BaseResponse(bool success, string message, int code = 0)
    {
        Success = success;
        Message = message;
        Code = code;
    }
}