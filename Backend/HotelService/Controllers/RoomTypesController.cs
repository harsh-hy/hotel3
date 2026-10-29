using HotelService.DTOs;
using HotelService.Services;
using HotelService.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Controllers;

[ApiController]
[Route("api/hotel/room-types")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypesController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [HttpGet]
    [Authorize(Roles = "Owner,Manager,Receptionist")]
    public async Task<IActionResult> GetAll()
    {
        var roomTypes = await _roomTypeService.GetAllAsync();
        return Ok(roomTypes);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Owner,Manager,Receptionist")]
    public async Task<IActionResult> GetById(int id)
    {
        var roomType = await _roomTypeService.GetByIdAsync(id);
        return Ok(roomType);
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateRoomTypeRequestDto request)
    {
        var created = await _roomTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.RoomTypeId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Owner,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomTypeRequestDto request)
    {
        var updated = await _roomTypeService.UpdateAsync(id, request);
        return Ok(updated);
    }
}
