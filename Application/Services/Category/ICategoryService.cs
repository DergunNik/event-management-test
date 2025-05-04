using Application.Dtos.Category;

namespace Application.Services.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryPageDto> GetEventsPageAsync(CategoryPaginationDto paginationDto, CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task AddCategoryAsync(CategoryCreateDto categoryDto, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(CategoryUpdateDto categoryDto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
}