using BookingService.Services;
using BookingService.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/booking/dashboard")]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var role = User.GetRole();
        var dashboard = await _dashboardService.GetDashboardAsync(role);
        return Ok(dashboard);
    }
}
