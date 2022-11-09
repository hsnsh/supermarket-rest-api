namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class ErrorDto
{
    public string Code { get; }
    public List<string> Messages { get; }

    public ErrorDto(List<string> messages, string code = null)
    {
        this.Code = code ?? string.Empty;
        this.Messages = messages ?? new List<string>();
    }

    public ErrorDto(string message, string code = null)
    {
        this.Code = code ?? string.Empty;
        this.Messages = new List<string>();

        if (!string.IsNullOrWhiteSpace(message))
        {
            this.Messages.Add(message);
        }
    }
}