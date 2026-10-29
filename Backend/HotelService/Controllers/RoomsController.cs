using HotelService.DTOs;
using HotelService.Services;
using HotelService.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers;

[ApiController]
[Route("api/hotel/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [Authorize(Roles = "Owner,Manager,Receptionist")]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? roomTypeId)
    {
        var rooms = await _roomService.GetAllAsync(status, roomTypeId);
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Owner,Manager,Receptionist")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        return Ok(room);
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequestDto request)
    {
        var created = await _roomService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomRequestDto request)
    {
        var updated = await _roomService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id:int}/price")]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdateRoomPriceRequestDto request)
    {
        var updated = await _roomService.UpdatePriceAsync(id, request);
        return Ok(updated);
    }
}
