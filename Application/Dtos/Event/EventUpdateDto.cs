namespace Application.Dtos.Event;

public class EventUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxParticipants { get; set; }
    public int CategoryId { get; set; }
}