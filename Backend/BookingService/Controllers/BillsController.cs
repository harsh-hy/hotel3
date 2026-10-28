using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/reservations")]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpGet("{id:int}/bill")]
    public async Task<IActionResult> GetBill(int id)
    {
        var bill = await _billService.GetBillForReservationAsync(id);
        return Ok(bill);
    }
}
