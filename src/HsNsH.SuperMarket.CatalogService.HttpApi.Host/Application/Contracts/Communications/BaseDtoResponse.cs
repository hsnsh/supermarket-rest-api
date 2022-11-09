namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public abstract class BaseDtoResponse<T> : BaseResponse
{
    public T Resource { get; }

    protected BaseDtoResponse(T resource) : base(success: true, message: string.Empty)
    {
        Resource = resource;
    }

    protected BaseDtoResponse(string message, int code = 0) : base(success: false, message, code)
    {
        Resource = default;
    }
}