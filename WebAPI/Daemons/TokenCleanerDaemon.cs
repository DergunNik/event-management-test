using Application.Options;
using Application.Services;
using Microsoft.Extensions.Options;

namespace WebAPI.Daemons;

public class TokenCleanerDaemon(
    ILogger<TokenCleanerDaemon> logger, 
    IOptions<TokensOptions> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TokenCleanerDaemon running.");

        using PeriodicTimer timer = new(TimeSpan.FromDays(options.Value.RefreshExpirationInDays));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await Clear(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("TokenCleanerDaemon is stopping.");
        }
        catch (Exception e)
        {
            logger.LogError("TokenCleanerDaemon unexpected err {e}", e.Message);
        }
    }

    private async Task Clear(CancellationToken token)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cleaner = scope.ServiceProvider.GetRequiredService<IRefreshTokenCleaner>();

        await cleaner.ClearAsync(token);
        
        logger.LogInformation("Refresh tokens are cleaned.");
    }
}