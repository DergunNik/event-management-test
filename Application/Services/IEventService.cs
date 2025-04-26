using System.Linq.Expressions;
using Application.Dtos.EventUsers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetEventsAsync(Expression<Func<Event, bool>> filter, CancellationToken cancellationToken = default);
    Task<Event?> GetEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task AddEventAsync(EventRequestDto eventRequestDto, CancellationToken cancellationToken);
    Task UpdateEventAsync(EventRequestDto eventRequestDto, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task SetEventImageAsync(int eventId, IFormFile image, CancellationToken cancellationToken = default);
    Task<string?> GetEventImageAsync(int eventId, CancellationToken cancellationToken = default);
}