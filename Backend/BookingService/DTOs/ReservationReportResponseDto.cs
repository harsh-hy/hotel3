namespace BookingService.DTOs;

public class ReservationReportResponseDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int TotalReservations { get; set; }
    public int TotalNightsBooked { get; set; }
    public decimal TotalRevenueBooked { get; set; }
    public Dictionary<string, int> StatusBreakdown { get; set; } = new();

    public ReservationReportResponseDto() { }

    public ReservationReportResponseDto(
        DateOnly? StartDate,
        DateOnly? EndDate,
        int TotalReservations,
        int TotalNightsBooked,
        decimal TotalRevenueBooked,
        Dictionary<string, int> StatusBreakdown)
    {
        this.StartDate = StartDate;
        this.EndDate = EndDate;
        this.TotalReservations = TotalReservations;
        this.TotalNightsBooked = TotalNightsBooked;
        this.TotalRevenueBooked = TotalRevenueBooked;
        this.StatusBreakdown = StatusBreakdown;
    }
}
