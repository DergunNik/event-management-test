using Application.Options;
using Application.Services;
using Microsoft.Extensions.Options;

namespace WebAPI.Daemons;

public class TokenCleanerDaemon : BackgroundService
{
    private readonly ILogger<TokenCleanerDaemon> _logger;
    private readonly IOptions<TokensOptions> _options;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TokenCleanerDaemon(
        ILogger<TokenCleanerDaemon> logger,
        IOptions<TokensOptions> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TokenCleanerDaemon running.");

        using PeriodicTimer timer = new(TimeSpan.FromDays(_options.Value.RefreshExpirationInDays));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken)) await Clear(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("TokenCleanerDaemon is stopping.");
        }
        catch (Exception e)
        {
            _logger.LogError("TokenCleanerDaemon unexpected err {e}", e.Message);
        }
    }

    private async Task Clear(CancellationToken token)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var cleaner = scope.ServiceProvider.GetRequiredService<IRefreshTokenCleaner>();

        await cleaner.ClearAsync(token);

        _logger.LogInformation("Refresh tokens are cleaned.");
    }
}