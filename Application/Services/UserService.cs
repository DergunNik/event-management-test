using System.Linq.Expressions;
using Application.Dtos.Participant;
using Application.Dtos.User;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Mapster;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetEventUsersAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var participants = 
            await unitOfWork.GetRepository<Participant>().ListAsync(p => p.EventId == eventId, cancellationToken);

        List<Task<User?>> userTasks = [];
        userTasks.AddRange(participants.Select(
            p => unitOfWork.GetRepository<User>().GetByIdAsync(p.UserId, cancellationToken)));

        await Task.WhenAll(userTasks);

        List<User> users = [];
        users.AddRange(userTasks.Select(ut => ut.Result).Where(ut => ut is not null));
        return users.Adapt<IEnumerable<UserDto>>();
    }

    public async Task<UserDto?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.GetRepository<User>().GetByIdAsync(userId, cancellationToken);
        return user?.Adapt<UserDto>();
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(Expression<Func<User, bool>> filter, CancellationToken cancellationToken = default)
    {
        var users = await unitOfWork.GetRepository<User>().ListAsync(filter, cancellationToken);
        return users.Adapt<IEnumerable<UserDto>>();
    }

    public async Task<UserPageDto> GetEventUsersPageAsync(int eventId, UserPaginationDto paginationDto,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Participant, bool>> filter = p => p.EventId == eventId;

        Func<IQueryable<Participant>, IOrderedQueryable<Participant>>? orderBy = paginationDto.SortBy switch
        {
            UserSortFields.Email => paginationDto.Descending
                ? q => q.OrderByDescending(p => p.User.Email)
                : q => q.OrderBy(p => p.User.Email),
            UserSortFields.DateOfBirth => paginationDto.Descending
                ? q => q.OrderByDescending(p => p.User.DateOfBirth)
                : q => q.OrderBy(p => p.User.DateOfBirth),
            UserSortFields.LastName => paginationDto.Descending
                ? q => q.OrderByDescending(p => p.User.LastName)
                : q => q.OrderBy(p => p.User.LastName),
            _ => null
        };

        var pagedResult = await unitOfWork
            .GetRepository<Participant>()
            .GetPagedAsync(
                paginationDto.Page,
                paginationDto.PageSize,
                filter,
                orderBy,
                cancellationToken,
                p => p.User
            );

        var users = pagedResult.Items
            .Select(p => p.User)
            .Where(u => u != null)
            .Adapt<List<UserDto>>();

        var result = pagedResult.Adapt<UserPageDto>();
        result.Users = users;
        
        return result;
    }

    public async Task<ManageParticipantResponse> AddParticipantAsync(int userId, int eventId, CancellationToken cancellationToken = default)
    {
        var (responce, _, @event, _) = await GetEntities(userId, eventId);
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
        var (responce, _, _, participant) = await GetEntities(userId, eventId);
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