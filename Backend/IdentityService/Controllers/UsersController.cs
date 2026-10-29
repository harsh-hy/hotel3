using IdentityService.DTOs;
using IdentityService.Services;
using IdentityService.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("api/identity/users")]
[Authorize(Roles = Roles.Owner)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request)
    {
        var createdUser = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.UserId }, createdUser);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequestDto request)
    {
        var updatedUser = await _userService.UpdateUserAsync(id, request);
        return Ok(updatedUser);
    }
}
