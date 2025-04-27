namespace Application.Dtos.Category;

public class CategoryPageDto
{
    public IEnumerable<CategoryDto> Categories { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}