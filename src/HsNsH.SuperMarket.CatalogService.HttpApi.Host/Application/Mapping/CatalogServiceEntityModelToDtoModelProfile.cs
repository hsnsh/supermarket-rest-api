using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Domain.Models;

namespace HsNsH.SuperMarket.CatalogService.Application.Mapping;

public class CatalogServiceEntityModelToDtoModelProfile : Profile
{
    public CatalogServiceEntityModelToDtoModelProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryWithNavigationsDto>()
            .ForMember(dest => dest.Products,
                opt =>
                    opt.MapFrom(src => src.Products));

        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductWithNavigationsDto>()
            .ForMember(dest => dest.Category,
                opt =>
                    opt.MapFrom(src => src.Category));
    }
}