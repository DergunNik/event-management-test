namespace Application.Dtos.Event;

public class EventFilterDto
{
    public string? Title { get; set; } = null;
    public DateTime? FromDate { get; set; } = null;
    public DateTime? ToDate { get; set; } = null;
    public string? Location { get; set; } = null;
    public int? CategoryId { get; set; } = null;
}