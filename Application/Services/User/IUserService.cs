﻿using System.Linq.Expressions;
using Application.Dtos.Participant;
using Application.Dtos.User;

namespace Application.Services.User;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetEventUsersAsync(int eventId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserDto>> GetUsersAsync(Expression<Func<Domain.Entities.User, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<UserPageDto> GetEventUsersPageAsync(int eventId, UserPaginationDto paginationDto,
        CancellationToken cancellationToken = default);

    Task<ManageParticipantResponse> AddParticipantAsync(int userId, int eventId,
        CancellationToken cancellationToken = default);

    Task<ManageParticipantResponse> RemoveParticipantAsync(int userId, int eventId,
        CancellationToken cancellationToken = default);
}