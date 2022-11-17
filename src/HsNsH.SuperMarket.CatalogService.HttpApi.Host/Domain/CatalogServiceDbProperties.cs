using JetBrains.Annotations;

namespace HsNsH.SuperMarket.CatalogService.Domain;

public static class CatalogServiceDbProperties
{
    public const string DbTablePrefix = "";

    [CanBeNull]
    public const string DbSchema = null;

    public const string ConnectionStringName = "CatalogService";
}