namespace BookingService.DTOs;

public class DashboardResponseDto
{
    public int TodayCheckIns { get; set; }
    public int TodayCheckOuts { get; set; }
    public int TotalActiveBookings { get; set; }
    public int TotalCheckedIn { get; set; }
    public decimal? TotalRevenue { get; set; }
    public decimal? OutstandingBalances { get; set; }
    public int? TotalReservations { get; set; }

    public DashboardResponseDto() { }

    public DashboardResponseDto(
        int TodayCheckIns,
        int TodayCheckOuts,
        int TotalActiveBookings,
        int TotalCheckedIn,
        decimal? TotalRevenue,
        decimal? OutstandingBalances,
        int? TotalReservations)
    {
        this.TodayCheckIns = TodayCheckIns;
        this.TodayCheckOuts = TodayCheckOuts;
        this.TotalActiveBookings = TotalActiveBookings;
        this.TotalCheckedIn = TotalCheckedIn;
        this.TotalRevenue = TotalRevenue;
        this.OutstandingBalances = OutstandingBalances;
        this.TotalReservations = TotalReservations;
    }
}
