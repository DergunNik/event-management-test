using Domain.Abstractions;
using Infrastructure.Data;
using Infrastructure.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AppDbContext>()
                .AddScoped<IUnitOfWork, EfcUnitOfWork>();
            return services;
        }
    }