using System.ComponentModel.DataAnnotations;

namespace HsNsH.SuperMarket.CatalogService.Domain.Models;

[Serializable]
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }
}

[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    protected Entity()
    {
    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    [Key]
    public TKey Id { get; private set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Id = {Id}";
    }
}