namespace HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;

public abstract class BaseDto
{
}

public abstract class BaseDto<TKey> : BaseDto
{
    public TKey Id { get; set; }
}