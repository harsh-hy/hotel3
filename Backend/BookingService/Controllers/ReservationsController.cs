using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/reservations")]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var reservations = await _reservationService.GetAllAsync(status);
        return Ok(reservations);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var reservations = await _reservationService.GetTodayReservationsAsync();
        return Ok(reservations);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        return Ok(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequestDto request)
    {
        var created = await _reservationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.ReservationId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationRequestDto request)
    {
        var updated = await _reservationService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var cancelled = await _reservationService.CancelAsync(id);
        return Ok(cancelled);
    }

    [HttpPost("{id:int}/check-in")]
    public async Task<IActionResult> CheckIn(int id)
    {
        var checkedIn = await _reservationService.CheckInAsync(id);
        return Ok(checkedIn);
    }

    [HttpPost("{id:int}/check-out")]
    public async Task<IActionResult> CheckOut(int id)
    {
        var checkedOut = await _reservationService.CheckOutAsync(id);
        return Ok(checkedOut);
    }
}
