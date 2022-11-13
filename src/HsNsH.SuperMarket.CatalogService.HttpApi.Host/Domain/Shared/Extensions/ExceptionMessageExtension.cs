namespace HsNsH.SuperMarket.CatalogService.Domain.Shared.Extensions;

public static class ExceptionMessageExtension
{
    public static IEnumerable<string> GetMessages(this Exception ex)
    {
        var messages = new List<string>();

        if (ex == null) return messages;
        var currentExc = ex;
        do
        {
            if (!string.IsNullOrWhiteSpace(currentExc.Message)) messages.Add(currentExc.Message);

            currentExc = currentExc.InnerException;
        } while (currentExc != null);

        return messages;
    }
}