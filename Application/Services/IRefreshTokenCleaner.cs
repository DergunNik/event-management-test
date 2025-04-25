namespace Application.Services;

public interface IRefreshTokenCleaner
{
    Task ClearAsync(CancellationToken token);
}