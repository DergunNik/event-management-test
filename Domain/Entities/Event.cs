namespace Domain.Entities;

public class Event
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime DateAndTime { get; set; }
    public int MaxParticipants { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    
    // public byte[] Image { get; set; }
}