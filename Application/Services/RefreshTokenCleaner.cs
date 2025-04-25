using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services;

public class RefreshTokenCleaner(IUnitOfWork unitOfWork) : IRefreshTokenCleaner
{
    public async Task ClearAsync(CancellationToken token)
    {
        await unitOfWork.GetRepository<RefreshToken>()
            .DeleteWhereAsync(rt => rt.ExpiresOnUtc < DateTime.UtcNow, token);
    }
}