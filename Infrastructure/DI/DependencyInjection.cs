using Application.Abstractions;
using Infrastructure.Data;
using Infrastructure.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<AppDbContext>()
            .AddScoped<IUnitOfWork, EfcUnitOfWork>();
        return services;
    }
}