using System.Linq.Expressions;
using Application.Dtos.Event;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class EventService(IUnitOfWork unitOfWork) : IEventService
{
    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    public async Task<IEnumerable<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        var events = await unitOfWork.GetRepository<Event>().ListAllAsync(cancellationToken);
        return events.Adapt<IEnumerable<EventDto>>();
    }

    public async Task<IEnumerable<EventDto>> GetEventsAsync(
        Expression<Func<Event, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        var events = await unitOfWork.GetRepository<Event>().ListAsync(filter, cancellationToken);
        return events.Adapt<IEnumerable<EventDto>>();
    }

    public async Task<EventPageDto> GetEventsPageAsync(Expression<Func<Event, bool>> filter, EventPaginationDto paginationDto,
        CancellationToken cancellationToken = default)
    {
        Func<IQueryable<Event>, IOrderedQueryable<Event>>? orderBy = paginationDto.SortBy switch
        {
            EventSortFields.Title => paginationDto.Descending
                ? q => q.OrderByDescending(e => e.Title)
                : q => q.OrderBy(e => e.Title),
            EventSortFields.Location => paginationDto.Descending
                ? q => q.OrderByDescending(e => e.Location)
                : q => q.OrderBy(e => e.Location),
            EventSortFields.DateTime => paginationDto.Descending
                ? q => q.OrderByDescending(e => e.DateTime)
                : q => q.OrderBy(e => e.DateTime),
            EventSortFields.MaxParticipants => paginationDto.Descending
                ? q => q.OrderByDescending(e => e.MaxParticipants)
                : q => q.OrderBy(e => e.MaxParticipants),
            _ => null
        };

        var pagedResult = await unitOfWork
            .GetRepository<Event>()
            .GetPagedAsync(
                paginationDto.Page,
                paginationDto.PageSize,
                filter,
                orderBy,
                cancellationToken
            );

        var events = pagedResult.Items.Adapt<List<EventDto>>();
        
        var result = pagedResult.Adapt<EventPageDto>();
        result.Events = events;

        return result;
    }
    public async Task<EventDto?> GetEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventId, cancellationToken);
        return @event?.Adapt<EventDto>();
    }

    public async Task AddEventAsync(EventCreateDto eventCreateDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventCreateDto.CategoryId, cancellationToken);
        
        var @event = eventCreateDto.Adapt<Event>();
        await unitOfWork.GetRepository<Event>().AddAsync(@event, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(EventUpdateDto eventUpdateDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventUpdateDto.CategoryId, cancellationToken);

        var fromBd = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventUpdateDto.Id, cancellationToken)
            ?? throw new NullReferenceException("Event not found.");
        eventUpdateDto.Adapt(fromBd);
        await unitOfWork.GetRepository<Event>().UpdateAsync(fromBd, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventId, cancellationToken)
            ?? throw new NullReferenceException("Event not found.");
        await unitOfWork.GetRepository<Event>().DeleteAsync(@event, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task SetEventImageAsync(int eventId, IFormFile image, CancellationToken cancellationToken = default)
    {
        var @event = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventId, cancellationToken)
            ?? throw new NullReferenceException("Event not found.");

        var fileName = $"event_{eventId}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(_imageDirectory, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream, cancellationToken);
        }

        @event.ImagePath = $"/images/{fileName}";
        await unitOfWork.GetRepository<Event>().UpdateAsync(@event, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<string?> GetEventImageAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventId, cancellationToken)
            ?? throw new NullReferenceException("Event not found.");

        return @event.ImagePath ?? null;
    }

    private async Task ThrowIfInvalidCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId, cancellationToken) == null)
        {
            throw new NullReferenceException("Invalid event category id.");
        }
    }
}