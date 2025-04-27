using Domain.Enums;

namespace Application.Dtos.User;

public class UserCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public UserRole UserRole { get; set; }
}