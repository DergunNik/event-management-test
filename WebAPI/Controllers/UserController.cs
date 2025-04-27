using Application.Dtos.Participant;
using Application.Dtos.User;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Versioning;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("events/{eventId:int}/participate")]
    public async Task<ActionResult<ManageParticipantResponse>> ParticipateInEvent(int eventId, 
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var result = await userService.AddParticipantAsync(userId, eventId, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("events/{eventId:int}/participate")]
    public async Task<ActionResult<ManageParticipantResponse>> CancelParticipation(int eventId, 
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var result = 
            await userService.RemoveParticipantAsync(userId, eventId, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("events/{eventId:int}/participants")]
    public async Task<ActionResult<UserPageDto>> GetEventParticipants(
        int eventId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] UserSortFields sortBy = UserSortFields.LastName,
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

        var pagination = new UserPaginationDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            Descending = descending
        };

        var result = await userService.GetEventUsersPageAsync(eventId, pagination, cancellationToken);
        return Ok(result);
    }

    [HttpGet("participants/{userId:int}")]
    public async Task<ActionResult<UserDto>> GetParticipantById(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}