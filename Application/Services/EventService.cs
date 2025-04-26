using System.Linq.Expressions;
using Application.Dtos.EventUsers;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class EventService(IUnitOfWork unitOfWork) : IEventService
{
    private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    public async Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.GetRepository<Event>().ListAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetEventsAsync(
        Expression<Func<Event, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await unitOfWork.GetRepository<Event>().ListAsync(filter, cancellationToken);
    }

    public async Task<Event?> GetEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.GetRepository<Event>().GetByIdAsync(eventId, cancellationToken);
    }

    public async Task AddEventAsync(EventRequestDto eventRequestDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventRequestDto.CategoryId, cancellationToken);
        
        var @event = eventRequestDto.Adapt<Event>();
        await unitOfWork.GetRepository<Event>().AddAsync(@event, cancellationToken);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(EventRequestDto eventRequestDto, CancellationToken cancellationToken = default)
    {
        await ThrowIfInvalidCategory(eventRequestDto.CategoryId, cancellationToken);

        var fromBd = await unitOfWork.GetRepository<Event>().GetByIdAsync(eventRequestDto.Id, cancellationToken)
            ?? throw new NullReferenceException("Event not found.");
        eventRequestDto.Adapt(fromBd);
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