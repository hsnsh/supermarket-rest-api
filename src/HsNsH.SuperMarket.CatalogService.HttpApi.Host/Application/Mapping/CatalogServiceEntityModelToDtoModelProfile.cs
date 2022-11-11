using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Domain.Models;

namespace HsNsH.SuperMarket.CatalogService.Application.Mapping;

public class CatalogServiceEntityModelToDtoModelProfile : Profile
{
    public CatalogServiceEntityModelToDtoModelProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}