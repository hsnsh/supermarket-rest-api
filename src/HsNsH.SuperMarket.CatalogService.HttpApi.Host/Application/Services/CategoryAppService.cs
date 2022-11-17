using System.Net;
using AutoMapper;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Communications;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Dtos;
using HsNsH.SuperMarket.CatalogService.Application.Contracts.Services;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Exceptions;
using HsNsH.SuperMarket.CatalogService.Domain.Shared.Extensions;

namespace HsNsH.SuperMarket.CatalogService.Application.Services;

public class CategoryAppService : BaseAppService, ICategoryAppService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryAppService> _logger;
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
            var itemList = await _categoryRepository.GetListAsync(sorting: nameof(Category.Name));
            if (itemList == null)
            {
                throw new DomainException("An error occurred while retrieving data");
            }

            return _mapper.Map<IEnumerable<Category>, List<CategoryDto>>(itemList);
        }
        catch (Exception ex)
        {
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
            if (id.Equals(Guid.Empty)) throw new BusinessException(message: $"Invalid id: {id.ToString()}");

            var category = await _categoryRepository.FindAsync(x => x.Id == id);

            return category == null
                ? new CategoryDtoResponse($"No record found with id {id.ToString()}", (int)HttpStatusCode.NotFound)
                : new CategoryDtoResponse(_mapper.Map<Category, CategoryDto>(category));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                BusinessException be => new CategoryDtoResponse(be.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                DomainException de => new CategoryDtoResponse(de.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }

    public async Task<CategoryDtoResponse> CreateAsync(CategoryCreateDto input)
    {
        try
        {
            if (input == null) throw new BusinessException(message: $"Input parameter can not be null");

            var newCategory = new Category { Name = input.Name };

            var createdCategory = await _categoryRepository.InsertAsync(newCategory, true);

            return new CategoryDtoResponse(_mapper.Map<Category, CategoryDto>(createdCategory));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                BusinessException be => new CategoryDtoResponse(be.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                DomainException de => new CategoryDtoResponse(de.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }

    public async Task<CategoryDtoResponse> UpdateAsync(Guid id, CategoryUpdateDto input)
    {
        try
        {
            if (id.Equals(Guid.Empty)) throw new BusinessException(message: $"Invalid id: {id.ToString()}");
            if (input == null) throw new BusinessException(message: $"Input parameter can not be null");

            var existingCategory = await _categoryRepository.FindAsync(x => x.Id == id);
            if (existingCategory == null) return new CategoryDtoResponse($"No record found with id {id.ToString()}", (int)HttpStatusCode.NotFound);

            existingCategory.Name = input.Name;

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory, true);

            return new CategoryDtoResponse(_mapper.Map<Category, CategoryDto>(updatedCategory));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                BusinessException be => new CategoryDtoResponse(be.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                DomainException de => new CategoryDtoResponse(de.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        try
        {
            if (id.Equals(Guid.Empty)) throw new BusinessException(message: $"Invalid id: {id.ToString()}");

            var category = await _categoryRepository.FindAsync(x => x.Id == id);
            if (category == null) return new BaseResponse(false, $"No record found with id {id.ToString()}", (int)HttpStatusCode.NotFound);

            await _categoryRepository.DeleteAsync(category, true);

            return new BaseResponse(true, "Successfully completed");
        }
        catch (Exception ex)
        {
            return ex switch
            {
                BusinessException be => new BaseResponse(false, be.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                DomainException de => new BaseResponse(false, de.GetMessages().ToList(), (int)HttpStatusCode.BadRequest),
                _ => throw new BusinessException("AppServiceException", "An error occurred while processing", ex)
            };
        }
    }
}