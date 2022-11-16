namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

public interface IEntity
{
    object[] GetKeys();
}

public interface IEntity<TKey> : IEntity
{
    TKey Id { get; }
}