namespace Application.Dtos.Auth;

public class RegistrationResponse
{
    public int UserId { get; set; }
    public bool IsEmailConfirmed { get; set; }
}