using Domain.Enums;

namespace Domain.Entities;

public class User : Entity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public UserRole UserRole { get; set; }
    public string PasswordHash { get; set; }
    
    public ICollection<EventParticipant> Events { get; set; }
}