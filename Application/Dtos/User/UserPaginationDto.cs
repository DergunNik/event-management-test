namespace Application.Dtos.User;

public class UserPaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public UserSortFields SortBy { get; set; }
    public bool Descending { get; set; }
}