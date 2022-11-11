using System.Net;
using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;

namespace HsNsH.SuperMarket.CatalogService.Application.Services;

public class CategoryAppService : BaseAppService, ICategoryAppService
{
    private readonly ILogger<CategoryAppService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryAppService(ILogger<CategoryAppService> logger
        , ICategoryRepository categoryRepository
        , IMapper mapper)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetListAsync()
    {
        try
        {
            var itemList = await _categoryRepository.GetListAsync();
            if (itemList == null)
            {
                throw new DomainException("An error occurred while retrieving data");
            }

            return _mapper.Map<IEnumerable<Category>, List<CategoryDto>>(itemList);
        }
        catch (Exception ex)
        {
            //return TestFunc<List<CategoryDto>>(ex);
            
            return ex switch
            {
                BusinessException be => throw be,
                DomainException de => throw new BusinessException(nameof(DomainException), de.Message, de.InnerException),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }

    public async Task<CategoryDtoResponse> GetByIdAsync(Guid id)
    {
        try
        {
            if (id.Equals(Guid.Empty)) throw new BusinessException(message: $"Invalid category id: {id.ToString()}");

            var category = await _categoryRepository.FindAsync(x => x.Id == id);

            return category == null
                ? new CategoryDtoResponse($"No record found with category id {id.ToString()}", (int)HttpStatusCode.NotFound)
                : new CategoryDtoResponse(_mapper.Map<Category, CategoryDto>(category));
        }
        catch (Exception ex)
        {
            //return TestFunc<CategoryDtoResponse>(ex);
            
            return ex switch
            {
                BusinessException be => new CategoryDtoResponse(be.Message, (int)HttpStatusCode.BadRequest),
                DomainException de => new CategoryDtoResponse(de.Message, (int)HttpStatusCode.BadRequest),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }

    public async Task<CategoryDtoResponse> CreateAsync(CategoryCreateDto input)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDtoResponse> UpdateAsync(Guid id, CategoryUpdateDto input)
    {
        throw new NotImplementedException();
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}