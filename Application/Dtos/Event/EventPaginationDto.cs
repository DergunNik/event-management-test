namespace Application.Dtos.Event;

public class EventPaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public EventSortFields SortBy { get; set; }
    public bool Descending { get; set; }
}