using System.Security.Claims;
using Application.Dtos.Auth;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest registrationDto)
    {
        try
        {
            var result = await authService.RegisterAsync(registrationDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("signin")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest loginDto)
    {
        try
        {
            var result = await authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest refreshDto)
    {
        try
        {
            var result = await authService.LoginWithRefreshAsync(refreshDto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("signout")]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        await authService.LogoutAsync(userId);
        return NoContent();
    }
}