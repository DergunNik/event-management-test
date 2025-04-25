namespace Application.Dtos.Auth;

public class AuthResponse
{
    public string AccessToken  { get; set; }
    public string RefreshToken { get; set; }
    public DateTime JwtExpiresOnUtc { get; set; }
    public DateTime RefreshExpiresOnUtc { get; set; }
}