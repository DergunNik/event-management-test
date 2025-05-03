using System.Linq.Expressions;
using Application.Dtos.Event;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<EventDto>> GetEventsAsync(Expression<Func<Event, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<EventPageDto> GetEventsPageAsync(EventFilterDto filter, EventPaginationDto paginationDto,
        CancellationToken cancellationToken = default);

    Task<EventDto?> GetEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task AddEventAsync(EventCreateDto eventCreateDto, CancellationToken cancellationToken = default);
    Task UpdateEventAsync(EventUpdateDto eventUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task SetEventImageAsync(int eventId, IFormFile image, CancellationToken cancellationToken = default);
    Task<string?> GetEventImageAsync(int eventId, CancellationToken cancellationToken = default);
}