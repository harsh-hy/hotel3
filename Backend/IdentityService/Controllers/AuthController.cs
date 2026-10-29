using IdentityService.DTOs;
using IdentityService.Services;
using IdentityService.Common.Exceptions;
using IdentityService.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/identity")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            throw new UnauthorizedException("User identity could not be verified.");
        }

        var user = await _authService.GetCurrentUserAsync(userId.Value);
        return Ok(user);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            throw new UnauthorizedException("User identity could not be verified.");
        }

        await _authService.ChangePasswordAsync(userId.Value, request);
        return Ok(new { message = "Password changed successfully." });
    }
}
