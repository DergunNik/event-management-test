using Application.Dtos.Auth;

namespace Application.Services;

public interface IAuthService
{
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationDto);
    Task<AuthResponse> LoginAsync(LoginRequest loginDto);
    Task<AuthResponse> LoginWithRefreshAsync(RefreshRequest refreshRequest);
    Task LogoutAsync(int userId);
}