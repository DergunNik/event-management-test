namespace Application.Dtos.Category;

public class CategoryPaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool Descending { get; set; }
}