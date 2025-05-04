using Application.Dtos.Auth;
using Application.Options;
using Application.Services.Auth.Helpers;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;
using Microsoft.Extensions.Options;

namespace Application.Services.Auth.Core;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TokensOptions _tokenOptions;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider,
        IPasswordHasher passwordHasher,
        IOptions<TokensOptions> tokenOptions)
    {
        _unitOfWork = unitOfWork;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
        _tokenOptions = tokenOptions.Value;
    }

    public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationDto)
    {
        if (await _unitOfWork.GetRepository<Domain.Entities.User>()
                .AnyAsync(u => u.Email == registrationDto.Email))
            throw new InvalidOperationException("Email is already registered.");

        var user = registrationDto.Adapt<Domain.Entities.User>();
        user.PasswordHash = await _passwordHasher.HashAsync(registrationDto.Password);
        await _unitOfWork.GetRepository<Domain.Entities.User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new RegistrationResponse
        {
            IsEmailConfirmed = user.IsEmailConfirmed,
            UserId = user.Id
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginDto)
    {
        var user = await _unitOfWork.GetRepository<Domain.Entities.User>()
                       .FirstOrDefaultAsync(u => u.Email == loginDto.Email)
                   ?? throw new ArgumentException("Invalid email or password.");

        if (!await _passwordHasher.VerifyAsync(loginDto.Password, user.PasswordHash))
            throw new ArgumentException("Invalid email or password.");

        var jwtToken = _tokenProvider.CreateJwt(user);
        var refreshToken = _tokenProvider.CreateRefresh();

        await _unitOfWork.GetRepository<RefreshToken>().AddAsync(
            new RefreshToken
            {
                Token = refreshToken,
                ExpiresOnUtc = RefreshExpires(),
                UserId = user.Id
            });
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken,
            JwtExpiresOnUtc = JwtExpires(),
            RefreshExpiresOnUtc = RefreshExpires()
        };
    }

    public async Task<AuthResponse> LoginWithRefreshAsync(RefreshRequest refreshRequest)
    {
        var repository = _unitOfWork.GetRepository<RefreshToken>();
        var token = await repository.FirstOrDefaultAsync(rt => rt.Token == refreshRequest.RefreshToken,
            includesProperties: rt => rt.User);

        if (token is null) throw new ArgumentException("Token is not valid.");

        if (token.ExpiresOnUtc < DateTime.UtcNow)
        {
            await LogoutAsync(token.UserId);
            throw new ArgumentException("Token is not valid.");
        }

        var newRefreshToken = _tokenProvider.CreateRefresh();
        var newJwtToken = _tokenProvider.CreateJwt(token.User);

        token.Token = newRefreshToken;
        token.ExpiresOnUtc = JwtExpires();

        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newJwtToken,
            RefreshToken = newRefreshToken,
            JwtExpiresOnUtc = JwtExpires(),
            RefreshExpiresOnUtc = RefreshExpires()
        };
    }

    public async Task LogoutAsync(int userId)
    {
        await _unitOfWork.GetRepository<RefreshToken>().DeleteWhereAsync(rt => rt.UserId == userId);
        await _unitOfWork.SaveChangesAsync();
    }

    private DateTime JwtExpires()
    {
        return DateTime.UtcNow.AddMinutes(_tokenOptions.JwtExpirationInMinutes);
    }

    private DateTime RefreshExpires()
    {
        return DateTime.UtcNow.AddDays(_tokenOptions.RefreshExpirationInDays);
    }
}