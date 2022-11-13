namespace HsNsH.SuperMarket.CatalogService.Domain.Shared.Consts;

public static class ProductConsts
{
    private const string DefaultSorting = "{0}Name asc";

    public static string GetDefaultSorting(bool withEntityName = false)
    {
        return string.Format(DefaultSorting, withEntityName ? $"{TableName}." : string.Empty);
    }

    public const string TableName = "Products";
    public const int NameMaxLength = 50;
}