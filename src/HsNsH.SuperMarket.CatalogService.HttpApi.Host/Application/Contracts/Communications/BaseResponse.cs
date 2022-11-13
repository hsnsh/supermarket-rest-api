namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;

public class BaseResponse : IBaseResponse
{
    public bool Success { get; }
    public List<string> Messages { get; }

    public int Code { get; }

    public BaseResponse(bool success, string message, int code = 0)
    {
        Success = success;
        Messages = new List<string>();
        if (!string.IsNullOrWhiteSpace(message))
        {
            Messages.Add(message);
        }

        Code = code;
    }

    public BaseResponse(bool success, IReadOnlyCollection<string> messages, int code = 0)
    {
        Success = success;
        Messages = messages == null ? new List<string>() : messages.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        Code = code;
    }
}