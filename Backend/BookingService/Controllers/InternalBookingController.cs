using BookingService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/internal")]
public class InternalBookingController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;

    public InternalBookingController(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    [HttpGet("reservations/conflicting-room-ids")]
    public async Task<IActionResult> GetConflictingRoomIds(
        [FromQuery] DateOnly checkIn, 
        [FromQuery] DateOnly checkOut)
    {
        var conflictingIds = await _reservationRepository.GetConflictingRoomIdsAsync(checkIn, checkOut);
        return Ok(conflictingIds);
    }

    [HttpGet("rooms/{roomId:int}/has-active-reservations")]
    public async Task<IActionResult> HasActiveReservations(int roomId)
    {
        var hasActive = await _reservationRepository.HasActiveReservationsForRoomAsync(roomId);
        return Ok(hasActive);
    }
}
