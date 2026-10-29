using BookingService.DTOs;
using BookingService.Repositories;
using BookingService.Common.Constants;

namespace BookingService.Services;

public interface IReportService
{
    Task<RevenueReportResponseDto> GetRevenueReportAsync(DateOnly? startDate, DateOnly? endDate);
    Task<ReservationReportResponseDto> GetReservationReportAsync(DateOnly? startDate, DateOnly? endDate);
}

public class ReportService : IReportService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IReservationRepository _reservationRepository;

    public ReportService(
        IPaymentRepository paymentRepository,
        IReservationRepository reservationRepository)
    {
        _paymentRepository = paymentRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<RevenueReportResponseDto> GetRevenueReportAsync(DateOnly? startDate, DateOnly? endDate)
    {
        var payments = (await _paymentRepository.GetPaymentsForReportAsync(startDate, endDate)).ToList();

        var totalRevenue = payments.Sum(p => p.Amount);
        var totalTransactions = payments.Count;

        var byMethod = new Dictionary<string, decimal>
        {
            [PaymentMethods.Cash] = payments.Where(p => p.PaymentMethod == PaymentMethods.Cash).Sum(p => p.Amount),
            [PaymentMethods.Card] = payments.Where(p => p.PaymentMethod == PaymentMethods.Card).Sum(p => p.Amount),
            [PaymentMethods.UPI] = payments.Where(p => p.PaymentMethod == PaymentMethods.UPI).Sum(p => p.Amount)
        };

        return new RevenueReportResponseDto(
            StartDate: startDate,
            EndDate: endDate,
            TotalRevenue: totalRevenue,
            TotalTransactions: totalTransactions,
            RevenueByPaymentMethod: byMethod
        );
    }

    public async Task<ReservationReportResponseDto> GetReservationReportAsync(DateOnly? startDate, DateOnly? endDate)
    {
        var reservations = (await _reservationRepository.GetReservationsForReportAsync(startDate, endDate)).ToList();

        var totalReservations = reservations.Count;
        var totalNights = reservations.Sum(r => 
            (r.CheckOutDate.ToDateTime(TimeOnly.MinValue) - r.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days);

        var totalRevenueBooked = reservations
            .Where(r => r.ReservationStatus != ReservationStatuses.Cancelled)
            .Sum(r => r.TotalAmount);

        var byStatus = new Dictionary<string, int>
        {
            [ReservationStatuses.Booked] = reservations.Count(r => r.ReservationStatus == ReservationStatuses.Booked),
            [ReservationStatuses.CheckedIn] = reservations.Count(r => r.ReservationStatus == ReservationStatuses.CheckedIn),
            [ReservationStatuses.CheckedOut] = reservations.Count(r => r.ReservationStatus == ReservationStatuses.CheckedOut),
            [ReservationStatuses.Cancelled] = reservations.Count(r => r.ReservationStatus == ReservationStatuses.Cancelled)
        };

        return new ReservationReportResponseDto(
            StartDate: startDate,
            EndDate: endDate,
            TotalReservations: totalReservations,
            TotalNightsBooked: totalNights,
            TotalRevenueBooked: totalRevenueBooked,
            StatusBreakdown: byStatus
        );
    }
}
