using System.Linq.Expressions;
using Application.Dtos.Category;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;

namespace Application.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await unitOfWork.GetRepository<Category>().ListAllAsync(cancellationToken);
        return categories.Adapt<IEnumerable<CategoryDto>>();
    }

    public async Task<CategoryPageDto> GetEventsPageAsync(
        Expression<Func<Category, bool>> filter, 
        CategoryPaginationDto paginationDto,
        CancellationToken cancellationToken = default)
    {
        Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = paginationDto.Descending
            ? q => q.OrderByDescending(c => c.Name)
            : q => q.OrderBy(c => c.Name);
        
        var pagedResult = await unitOfWork
            .GetRepository<Category>()
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
        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId, cancellationToken);
        return category?.Adapt<CategoryDto>();
    }

    public async Task AddCategoryAsync(CategoryCreateDto categoryDto, CancellationToken cancellationToken = default)
    {
        var category = categoryDto.Adapt<Category>();
        await unitOfWork.GetRepository<Category>().AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(CategoryUpdateDto categoryDto, CancellationToken cancellationToken = default)
    {
        var fromDb = await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryDto.Id, cancellationToken)
            ?? throw new NullReferenceException("Category not found.");
        
        categoryDto.Adapt(fromDb);
        await unitOfWork.GetRepository<Category>().UpdateAsync(fromDb, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId, cancellationToken)
            ?? throw new NullReferenceException("Category not found.");
        
        await unitOfWork.GetRepository<Category>().DeleteAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }
}