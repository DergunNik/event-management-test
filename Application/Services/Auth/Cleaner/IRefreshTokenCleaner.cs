namespace Application.Services.Auth.Cleaner;

public interface IRefreshTokenCleaner
{
    Task ClearAsync(CancellationToken token);
}