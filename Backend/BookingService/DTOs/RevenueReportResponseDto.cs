namespace BookingService.DTOs;

public class RevenueReportResponseDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public Dictionary<string, decimal> RevenueByPaymentMethod { get; set; } = new();

    public RevenueReportResponseDto() { }

    public RevenueReportResponseDto(
        DateOnly? StartDate,
        DateOnly? EndDate,
        decimal TotalRevenue,
        int TotalTransactions,
        Dictionary<string, decimal> RevenueByPaymentMethod)
    {
        this.StartDate = StartDate;
        this.EndDate = EndDate;
        this.TotalRevenue = TotalRevenue;
        this.TotalTransactions = TotalTransactions;
        this.RevenueByPaymentMethod = RevenueByPaymentMethod;
    }
}
