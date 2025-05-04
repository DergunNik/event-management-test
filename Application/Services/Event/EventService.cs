using System.Linq.Expressions;
using Application.Dtos.Event;
using Domain.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Event;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    public async Task<IEnumerable<EventDto>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.GetRepository<Domain.Entities.Event>().ListAllAsync(cancellationToken);
        return events.Adapt<IEnumerable<EventDto>>();
    }

    public async Task<IEnumerable<EventDto>> GetEventsAsync(
        Expression<Func<Domain.Entities.Event, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.GetRepository<Domain.Entities.Event>().ListAsync(filter, cancellationToken);
        return events.Adapt<IEnumerable<EventDto>>();
    }

    public async Task<EventPageDto> GetEventsPageAsync(EventFilterDto filterDto, EventPaginationDto paginationDto,
        CancellationToken cancellationToken = default)
    {
        var filter = CreateFilter(filterDto);
        var orderBy = CreateOrderBy(paginationDto);

        var pagedResult = await _unitOfWork
            .GetRepository<Domain.Entities.Event>()
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
        var @event = await _unitOfWork.GetRepository<Domain.Entities.Event>().GetByIdAsync(eventId, cancellationToken);
        return @event?.Adapt<EventDto>();
    }

    public async Task AddEventAsync(EventCreateDto eventCreateDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventCreateDto.CategoryId, cancellationToken);

        var @event = eventCreateDto.Adapt<Domain.Entities.Event>();
        await _unitOfWork.GetRepository<Domain.Entities.Event>().AddAsync(@event, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(EventUpdateDto eventUpdateDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventUpdateDto.CategoryId, cancellationToken);

        var fromBd = await _unitOfWork.GetRepository<Domain.Entities.Event>().GetByIdAsync(eventUpdateDto.Id, cancellationToken)
                     ?? throw new NullReferenceException("Event not found.");
        eventUpdateDto.Adapt(fromBd);
        await _unitOfWork.GetRepository<Domain.Entities.Event>().UpdateAsync(fromBd, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _unitOfWork.GetRepository<Domain.Entities.Event>().GetByIdAsync(eventId, cancellationToken)
                     ?? throw new NullReferenceException("Event not found.");
        await _unitOfWork.GetRepository<Domain.Entities.Event>().DeleteAsync(@event, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SetEventImageAsync(int eventId, IFormFile image, CancellationToken cancellationToken = default)
    {
        var @event = await _unitOfWork.GetRepository<Domain.Entities.Event>().GetByIdAsync(eventId, cancellationToken)
                     ?? throw new NullReferenceException("Event not found.");

        var fileName = $"event_{eventId}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(_imageDirectory, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream, cancellationToken);
        }

        @event.ImagePath = $"/images/{fileName}";
        await _unitOfWork.GetRepository<Domain.Entities.Event>().UpdateAsync(@event, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string?> GetEventImageAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _unitOfWork.GetRepository<Domain.Entities.Event>().GetByIdAsync(eventId, cancellationToken)
                     ?? throw new NullReferenceException("Event not found.");

        return @event.ImagePath ?? null;
    }

    private static Func<IQueryable<Domain.Entities.Event>, IOrderedQueryable<Domain.Entities.Event>>? CreateOrderBy(EventPaginationDto paginationDto)
    {
        return paginationDto.SortBy switch
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
    }

    private static Expression<Func<Domain.Entities.Event, bool>> CreateFilter(EventFilterDto filterDto)
    {
        return e =>
            (filterDto.Title == null || e.Title.Contains(filterDto.Title)) &&
            (filterDto.FromDate == null || e.DateTime >= filterDto.FromDate) &&
            (filterDto.ToDate == null || e.DateTime <= filterDto.ToDate) &&
            (filterDto.Location == null || e.Location.Contains(filterDto.Location)) &&
            (filterDto.CategoryId == null || e.CategoryId == filterDto.CategoryId);
    }
    
    private async Task ThrowIfInvalidCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<Domain.Entities.Category>().GetByIdAsync(categoryId, cancellationToken) == null)
            throw new NullReferenceException("Invalid event category id.");
    }
}