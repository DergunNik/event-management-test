using Application.Dtos.Auth;

namespace Application.Services.Auth.Core;

public interface IAuthService
{
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationDto);
    Task<AuthResponse> LoginAsync(LoginRequest loginDto);
    Task<AuthResponse> LoginWithRefreshAsync(RefreshRequest refreshRequest);
    Task LogoutAsync(int userId);
}