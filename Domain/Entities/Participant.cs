namespace Domain.Entities;

public class Participant : Entity
{
    public int EventId { get; set; }
    public Event Event { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    
    public DateTime RegistrationDate { get; set; }
}