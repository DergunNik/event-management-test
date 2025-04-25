using Application.Dtos.Auth;

namespace Application.Services;

public interface IAuthService
{
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationDto);
    Task<AuthResponse> LoginAsync(LoginRequest loginDto);
    Task<AuthResponse> LoginWithRefreshAsync(string refreshToken);
    Task LogoutAsync(int userId);
}