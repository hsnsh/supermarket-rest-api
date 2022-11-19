using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;

namespace HsNsH.SuperMarket.CatalogService.Application.Services;

public class DataAppService : IDataAppService
{
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _repository;

    public DataAppService(IMapper mapper, ICategoryRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetListAsync()
    {
        var datas = await _repository.GetListAsync();

        return _mapper.Map<IEnumerable<Category>, List<CategoryDto>>(datas);
    }

    public async Task<List<CategoryWithNavigationsDto>> GetListWithNavigationsAsync()
    {
        var datas = await _repository.GetListAsync(includeDetails: true);

        return _mapper.Map<IEnumerable<Category>, List<CategoryWithNavigationsDto>>(datas);
    }
}