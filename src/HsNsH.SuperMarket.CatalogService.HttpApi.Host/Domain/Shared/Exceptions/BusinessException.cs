namespace HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; set; }

    public BusinessException(string code = null, string message = null, Exception innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    public BusinessException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}