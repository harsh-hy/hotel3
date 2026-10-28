using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/guests")]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var guests = await _guestService.GetAllAsync(search);
        return Ok(guests);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var guest = await _guestService.GetByIdAsync(id);
        return Ok(guest);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuestRequestDto request)
    {
        var created = await _guestService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.GuestId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGuestRequestDto request)
    {
        var updated = await _guestService.UpdateAsync(id, request);
        return Ok(updated);
    }
}
