namespace Domain.Entities;

public class Event : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime DateTime { get; set; }
    public int MaxParticipants { get; set; }
    public string? ImagePath { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
}