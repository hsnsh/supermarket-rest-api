namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public abstract class BaseDtoResponse<T> : BaseResponse
{
    public T Resource { get; }

    protected BaseDtoResponse(T resource) : base(message: string.Empty, hasError: false)
    {
        Resource = resource;
    }

    protected BaseDtoResponse(string message, bool isInternalError = false) : base(message, hasError: true, isInternalError)
    {
        Resource = default;
    }
}