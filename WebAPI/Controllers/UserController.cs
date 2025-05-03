using System.Security.Claims;
using Application.Dtos.Participant;
using Application.Dtos.User;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("events/{eventId:int}/participate")]
    public async Task<ActionResult<ManageParticipantResponse>> ParticipateInEvent(int eventId,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId)) return Unauthorized();

        var result = await _userService.AddParticipantAsync(userId, eventId, cancellationToken);

        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("events/{eventId:int}/participate")]
    public async Task<ActionResult<ManageParticipantResponse>> CancelParticipation(int eventId,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId)) return Unauthorized();

        var result = await _userService.RemoveParticipantAsync(userId, eventId, cancellationToken);

        if (!result.Success) return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("events/{eventId:int}/participants")]
    public async Task<ActionResult<UserPageDto>> GetEventParticipants(int eventId,
        [FromQuery] UserPaginationDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetEventUsersPageAsync(eventId, dto, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserAsync(userId, cancellationToken);
        if (user is null) return NotFound();

        return Ok(user);
    }
}