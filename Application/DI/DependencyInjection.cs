using Application.Options;
using Application.Services;
using Application.Services.Auth.Cleaner;
using Application.Services.Auth.Core;
using Application.Services.Auth.Helpers;
using Application.Services.Category;
using Application.Services.Event;
using Application.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"))
            .Configure<HashOptions>(configuration.GetSection("HashOptions"))
            .Configure<TokensOptions>(configuration.GetSection("TokensOptions"))
            .Configure<ContentRestrictions>(configuration.GetSection("ContentRestrictions"))
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IPasswordHasher, Argon2PasswordHasher>()
            .AddScoped<ITokenProvider, TokenProvider>()
            .AddScoped<IRefreshTokenCleaner, RefreshTokenCleaner>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<IEventService, EventService>();
        return services;
    }
}