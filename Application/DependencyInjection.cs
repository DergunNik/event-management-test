using Application.Options;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"))
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IPasswordHasher, Argon2PasswordHasher>()
                .AddScoped<ITokenProvider, TokenProvider>();
        return services;
    }
}