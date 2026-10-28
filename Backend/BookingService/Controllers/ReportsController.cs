using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/reports")]
[Authorize(Roles = "Owner,Manager")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport(
        [FromQuery] DateOnly? startDate, 
        [FromQuery] DateOnly? endDate)
    {
        var report = await _reportService.GetRevenueReportAsync(startDate, endDate);
        return Ok(report);
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> GetReservationReport(
        [FromQuery] DateOnly? startDate, 
        [FromQuery] DateOnly? endDate)
    {
        var report = await _reportService.GetReservationReportAsync(startDate, endDate);
        return Ok(report);
    }
}
