using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Domain.Models;

namespace HsNsH.SuperMarket.CatalogService.Application.Mapping;

public class CatalogServiceDtoModelToEntityModelProfile : Profile
{
    public CatalogServiceDtoModelToEntityModelProfile()
    {
        CreateMap<CategoryDto, Category>();
        CreateMap<CategoryWithNavigationsDto, Category>();

        CreateMap<ProductDto, Product>();
        CreateMap<ProductWithNavigationsDto, Product>();
    }
}