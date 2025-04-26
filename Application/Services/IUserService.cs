using Application.Dtos.EventUsers;
using Domain.Entities;

namespace Application.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetEventUsersAsync(int eventId, CancellationToken cancellationToken = default);
    Task<User?> GetUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<ManageParticipantResponse> AddParticipantAsync(int userId, int eventId, CancellationToken cancellationToken = default);
    Task<ManageParticipantResponse> RemoveParticipantAsync(int userId, int eventId, CancellationToken cancellationToken = default);
}