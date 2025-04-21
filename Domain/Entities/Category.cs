namespace Domain.Entities;

public class Category : Entity
{
    public string Name { get; set; }
    public ICollection<Event> Events { get; set; }
}