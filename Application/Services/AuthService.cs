using System.Security.Claims;
using Application.Dtos.Auth;
using Application.Options;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;
using Microsoft.Extensions.Options;


namespace Application.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    ITokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    IOptions<TokensOptions> tokenOptions) : IAuthService
{
    public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationDto)
    {
        if (await unitOfWork.GetRepository<User>()
                .AnyAsync(u => u.Email == registrationDto.Email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = registrationDto.Adapt<User>();
        await unitOfWork.GetRepository<User>().AddAsync(user);
        await unitOfWork.SaveChangesAsync();
        
        return new RegistrationResponse()
        {
            IsEmailConfirmed = user.IsEmailConfirmed,
            UserId = user.Id
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginDto)
    {
        var user = await unitOfWork.GetRepository<User>()
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email)
            ?? throw new ArgumentException("Invalid email or password.");
        
        if (!await passwordHasher.VerifyAsync(loginDto.Password, user.PasswordHash))
        {
            throw new ArgumentException("Invalid email or password.");
        }

        var jwtToken = tokenProvider.CreateJwt(user);
        var refreshToken = tokenProvider.CreateRefresh();

        await unitOfWork.GetRepository<RefreshToken>().AddAsync(
            new RefreshToken()
            {
                Token = refreshToken,
                ExpiresOnUtc = RefreshExpires(),
                UserId = user.Id
            });
        await unitOfWork.SaveChangesAsync();

        return new AuthResponse()
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken,
            JwtExpiresOnUtc = JwtExpires(),
            RefreshExpiresOnUtc = RefreshExpires()
        };
    }

    public async Task<AuthResponse> LoginWithRefreshAsync(string refreshToken)
    {
        var repository = unitOfWork.GetRepository<RefreshToken>();
        var token = await repository.FirstOrDefaultAsync(rt => rt.Token == refreshToken,
            includesProperties: rt => rt.User);
        
        if (token is null)
        {
            throw new ArgumentException("Token is not valid.");
        }

        if (token.ExpiresOnUtc < DateTime.UtcNow)
        {
            await LogoutAsync(token.UserId);
            throw new ArgumentException("Token is not valid.");
        }

        var newRefreshToken = tokenProvider.CreateRefresh();
        var newJwtToken = tokenProvider.CreateJwt(token.User);
        
        token.Token = newRefreshToken;
        token.ExpiresOnUtc = JwtExpires();
        
        await unitOfWork.SaveChangesAsync();

        return new AuthResponse()
        {
            AccessToken = newJwtToken,
            RefreshToken = newRefreshToken,
            JwtExpiresOnUtc = JwtExpires(),
            RefreshExpiresOnUtc = RefreshExpires()
        };
    }

    public async Task LogoutAsync(int userId)
    {
        await unitOfWork.GetRepository<RefreshToken>().DeleteWhereAsync(rt => rt.UserId == userId);
        await unitOfWork.SaveChangesAsync();
    }

    private DateTime JwtExpires()
    {
        return DateTime.UtcNow.AddMinutes(tokenOptions.Value.JwtExpirationInMinutes);
    }
    
    private DateTime RefreshExpires()
    {
        return DateTime.UtcNow.AddDays(tokenOptions.Value.RefreshExpirationInDays);
    }
}