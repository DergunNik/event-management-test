using Application.Dtos.EventUsers;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public async Task<IEnumerable<User>> GetEventUsersAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var participants = 
            await unitOfWork.GetRepository<Participant>().ListAsync(p => p.EventId == eventId, cancellationToken);

        List<Task<User?>> userTasks = [];
        userTasks.AddRange(participants.Select(
            p => unitOfWork.GetRepository<User>().GetByIdAsync(p.UserId, cancellationToken)));

        Task.WaitAll(userTasks, cancellationToken);

        List<User> users = [];
        users.AddRange(userTasks.Select(ut => ut.Result).Where(ut => ut is not null));
        return users;
    }

    public async Task<User?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.GetRepository<User>().GetByIdAsync(userId, cancellationToken);
    }

    public async Task<ManageParticipantResponse> AddParticipantAsync(int userId, int eventId, CancellationToken cancellationToken = default)
    {
        var (responce, user, @event, _) = await GetEntities(userId, eventId);
        if (responce is not null)
        {
            return responce;
        }
        
        var participants = await unitOfWork.GetRepository<Participant>()
            .ListAsync(p => p.EventId == eventId, cancellationToken);

        if (@event.MaxParticipants < participants.Count)
        {
            return new ManageParticipantResponse(false, "Already max participant number");
        }
        
        var participant = new Participant
        {
            EventId = eventId,
            UserId = userId,
            RegistrationDate = DateTime.UtcNow
        };
        await unitOfWork.GetRepository<Participant>().AddAsync(participant, cancellationToken);
        await unitOfWork.SaveChangesAsync();
        return new ManageParticipantResponse(true);
    }

    public async Task<ManageParticipantResponse> RemoveParticipantAsync(int userId, int eventId, CancellationToken cancellationToken = default)
    {
        var (responce, user, @event, participant) = await GetEntities(userId, eventId);
        if (responce is not null)
        {
            return responce;
        }

        if (participant is null)
        {
            return new ManageParticipantResponse(true);
        }

        await unitOfWork.GetRepository<Participant>().DeleteAsync(participant, cancellationToken);
        await unitOfWork.SaveChangesAsync();
        return new ManageParticipantResponse(true);
    }

    private async Task<(ManageParticipantResponse?, User?, Event?, Participant?)> GetEntities(int userId, int eventId)
    {
        var participantTask = unitOfWork.GetRepository<Participant>()
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        var userTask = unitOfWork.GetRepository<User>().GetByIdAsync(userId);
        var eventTask = unitOfWork.GetRepository<Event>().GetByIdAsync(eventId);

        await Task.WhenAll(userTask, eventTask, participantTask);

        var user = await userTask;
        if (user is null || user.UserRole != UserRole.DefaultUser)
        {
            return (new ManageParticipantResponse(false, "Invalid user id"), null, null, null);
        }
        
        var @event = await eventTask;
        if (@event is null) 
        {
            return (new ManageParticipantResponse(false, "Invalid event id"), null, null, null);
        }

        return (null, user, @event, await participantTask);
    }
}