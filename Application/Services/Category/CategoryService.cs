using System.Linq.Expressions;
using Application.Dtos.Category;
using Domain.Abstractions;
using Mapster;

namespace Application.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.GetRepository<Domain.Entities.Category>().ListAllAsync(cancellationToken);
        return categories.Adapt<IEnumerable<CategoryDto>>();
    }

    public async Task<CategoryPageDto> GetEventsPageAsync(CategoryPaginationDto paginationDto,
        CancellationToken cancellationToken = default)
    {
        Func<IQueryable<Domain.Entities.Category>, IOrderedQueryable<Domain.Entities.Category>>? orderBy = paginationDto.Descending
            ? q => q.OrderByDescending(c => c.Name)
            : q => q.OrderBy(c => c.Name);

        Expression<Func<Domain.Entities.Category, bool>> filter = string.IsNullOrWhiteSpace(paginationDto.Search)
            ? _ => true
            : c => c.Name.Contains(paginationDto.Search);
        
        var pagedResult = await _unitOfWork
            .GetRepository<Domain.Entities.Category>()
            .GetPagedAsync(
                paginationDto.Page,
                paginationDto.PageSize,
                filter,
                orderBy,
                cancellationToken
            );

        var categories = pagedResult.Items.Adapt<List<CategoryDto>>();

        var result = pagedResult.Adapt<CategoryPageDto>();
        result.Categories = categories;

        return result;
    }

    public async Task<CategoryDto?> GetCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.GetRepository<Domain.Entities.Category>().GetByIdAsync(categoryId, cancellationToken);
        return category?.Adapt<CategoryDto>();
    }

    public async Task AddCategoryAsync(CategoryCreateDto categoryDto, CancellationToken cancellationToken = default)
    {
        var category = categoryDto.Adapt<Domain.Entities.Category>();
        await _unitOfWork.GetRepository<Domain.Entities.Category>().AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(CategoryUpdateDto categoryDto, CancellationToken cancellationToken = default)
    {
        var fromDb = await _unitOfWork.GetRepository<Domain.Entities.Category>().GetByIdAsync(categoryDto.Id, cancellationToken)
                     ?? throw new NullReferenceException("Category not found.");

        categoryDto.Adapt(fromDb);
        await _unitOfWork.GetRepository<Domain.Entities.Category>().UpdateAsync(fromDb, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.GetRepository<Domain.Entities.Category>().GetByIdAsync(categoryId, cancellationToken)
                       ?? throw new NullReferenceException("Category not found.");

        await _unitOfWork.GetRepository<Domain.Entities.Category>().DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }
}