namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class ErrorDto
{
    public string Code { get; }
    public List<string> Messages { get; }

    public ErrorDto(IReadOnlyCollection<string> messages, string code = null)
    {
        Code = code ?? string.Empty;
        Messages = messages == null ? new List<string>() : messages.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public ErrorDto(string message, string code = null)
    {
        Code = code ?? string.Empty;
        Messages = new List<string>();

        if (!string.IsNullOrWhiteSpace(message))
        {
            Messages.Add(message);
        }
    }
}