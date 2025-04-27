using Application.Dtos.Event;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Asp.Versioning;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EventController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EventPageDto>> GetAllEventsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] EventSortFields sortBy = EventSortFields.DateTime,
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0.");
        }
        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0.");
        }

        var pagination = new EventPaginationDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            Descending = descending
        };

        Expression<Func<Event, bool>> filter = _ => true;
        var eventsPage = await eventService.GetEventsPageAsync(filter, pagination, cancellationToken);
        return Ok(eventsPage);
    }

    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventDto>> GetEventById(int eventId, CancellationToken cancellationToken = default)
    {
        var ev = await eventService.GetEventAsync(eventId, cancellationToken);
        if (ev is null)
        {
            return NotFound();
        }

        return Ok(ev);
    }

    [HttpGet("title/{title}")]
    public async Task<ActionResult<EventPageDto>> GetEventsByTitlePaged(
        string title,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] EventSortFields sortBy = EventSortFields.DateTime,
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest("Page number must be greater than 0.");
        }
        if (pageSize < 1)
        {
            return BadRequest("Page size must be greater than 0.");
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Title cannot be empty.");
        }

        var pagination = new EventPaginationDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            Descending = descending
        };

        Expression<Func<Event, bool>> filter = e => e.Title.Contains(title);

        var eventsPage = await eventService.GetEventsPageAsync(filter, pagination, cancellationToken);
        return Ok(eventsPage);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<EventPageDto>> GetEventsByFilterPaged(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? location,
        [FromQuery] int? categoryId,
        [FromBody] EventPaginationDto pagination,
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
        {
            return BadRequest("fromDate must be less than or equal to toDate.");
        }

        Expression<Func<Event, bool>> filter = e =>
            (!fromDate.HasValue || e.DateTime.Date >= fromDate.Value.Date) &&
            (!toDate.HasValue || e.DateTime.Date <= toDate.Value.Date) &&
            (string.IsNullOrEmpty(location) || e.Location.Contains(location)) &&
            (!categoryId.HasValue || e.CategoryId == categoryId);

        var eventsPage = await eventService.GetEventsPageAsync(filter, pagination, cancellationToken);
        return Ok(eventsPage);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] EventCreateDto dto, CancellationToken cancellationToken = default)
    {
        await eventService.AddEventAsync(dto, cancellationToken);
        return StatusCode(201);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await eventService.UpdateEventAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{eventId:int}")]
    public async Task<IActionResult> DeleteEvent(int eventId, CancellationToken cancellationToken = default)
    {
        await eventService.DeleteEventAsync(eventId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{eventId:int}/image")]
    public async Task<IActionResult> UploadEventImage(int eventId, IFormFile image, CancellationToken cancellationToken = default)
    {
        if (image.Length == 0)
        {
            return BadRequest("No image provided.");
        }

        await eventService.SetEventImageAsync(eventId, image, cancellationToken);
        return Ok();
    }

    [HttpGet("{eventId:int}/image")]
    public async Task<ActionResult<string>> GetEventImage(int eventId, CancellationToken cancellationToken = default)
    {
        var path = await eventService.GetEventImageAsync(eventId, cancellationToken);
        if (path is null)
        {
            return NotFound();
        }

        return Ok(path);
    }
}