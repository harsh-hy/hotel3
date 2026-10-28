using HotelService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers;

[ApiController]
[Route("api/hotel/rooms")]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> SearchAvailable(
        [FromQuery] DateOnly checkIn, 
        [FromQuery] DateOnly checkOut, 
        [FromQuery] int adults = 1, 
        [FromQuery] int children = 0)
    {
        var rooms = await _availabilityService.SearchAvailableAsync(checkIn, checkOut, adults, children);
        return Ok(rooms);
    }

    [HttpGet("{id:int}/availability")]
    public async Task<IActionResult> CheckAvailability(
        int id, 
        [FromQuery] DateOnly checkIn, 
        [FromQuery] DateOnly checkOut)
    {
        var result = await _availabilityService.CheckRoomAvailabilityAsync(id, checkIn, checkOut);
        return Ok(result);
    }
}
