using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services.Auth.Cleaner;

public class RefreshTokenCleaner : IRefreshTokenCleaner
{
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCleaner(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task ClearAsync(CancellationToken token)
    {
        await _unitOfWork.GetRepository<RefreshToken>()
            .DeleteWhereAsync(rt => rt.ExpiresOnUtc < DateTime.UtcNow, token);
    }
}