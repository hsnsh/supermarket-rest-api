namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public class ErrorDto
{
    public List<string> Messages { get; private set; }

    public ErrorDto(List<string> messages)
    {
        this.Messages = messages ?? new List<string>();
    }

    public ErrorDto(string message)
    {
        this.Messages = new List<string>();

        if(!string.IsNullOrWhiteSpace(message))
        {
            this.Messages.Add(message);
        }
    }
}