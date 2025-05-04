using System.Linq.Expressions;
using Application.Dtos.Event;
using Application.Services;
using Application.Services.Event;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpGet]
    public async Task<ActionResult<EventPageDto>> GetAllEventsPaged([FromQuery] EventPaginationDto dto,
        CancellationToken cancellationToken = default)
    {
        var filter = new EventFilterDto();
        var eventsPage = await _eventService.GetEventsPageAsync(filter, dto, cancellationToken);
        return Ok(eventsPage);
    }

    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<EventDto>> GetEventById(int eventId, CancellationToken cancellationToken = default)
    {
        var ev = await _eventService.GetEventAsync(eventId, cancellationToken);
        if (ev is null) return NotFound();
        return Ok(ev);
    }

    [HttpGet("title/{title}")]
    public async Task<ActionResult<EventPageDto>> GetEventsByTitlePaged(
        string title,
        [FromQuery] EventPaginationDto dto,
        CancellationToken cancellationToken = default)
    {
        var filter = new EventFilterDto{Title = title};
        var eventsPage = await _eventService.GetEventsPageAsync(filter, dto, cancellationToken);
        return Ok(eventsPage);
    }

    [HttpGet("filter")]
    public async Task<ActionResult<EventPageDto>> GetEventsByFilterPaged(
        [FromQuery] EventPaginationDto paginationDto,
        [FromQuery] EventFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var eventsPage = await _eventService.GetEventsPageAsync(filterDto, paginationDto, cancellationToken);
        return Ok(eventsPage);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddEvent([FromBody] EventCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        await _eventService.AddEventAsync(dto, cancellationToken);
        return StatusCode(201);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        await _eventService.UpdateEventAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{eventId:int}")]
    public async Task<IActionResult> DeleteEvent(int eventId, CancellationToken cancellationToken = default)
    {
        await _eventService.DeleteEventAsync(eventId, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{eventId:int}/image")]
    public async Task<IActionResult> UploadEventImage(int eventId, IFormFile image,
        CancellationToken cancellationToken = default)
    {
        if (image.Length == 0) return BadRequest("No image provided.");
        
        await _eventService.SetEventImageAsync(eventId, image, cancellationToken);
        return Ok();
    }

    [HttpGet("{eventId:int}/image")]
    public async Task<ActionResult<string>> GetEventImage(int eventId, CancellationToken cancellationToken = default)
    {
        var path = await _eventService.GetEventImageAsync(eventId, cancellationToken);
        if (path is null) return NotFound();

        return Ok(path);
    }
}