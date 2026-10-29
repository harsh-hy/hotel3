using BookingService.DTOs;
using BookingService.Repositories;
using BookingService.Common.Constants;

namespace BookingService.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync(string? userRole);
}

public class DashboardService : IDashboardService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;

    public DashboardService(
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<DashboardResponseDto> GetDashboardAsync(string? userRole)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var allReservations = (await _reservationRepository.GetAllAsync()).ToList();

        var todayCheckIns = allReservations.Count(r => 
            r.CheckInDate == today && r.ReservationStatus == ReservationStatuses.Booked);

        var todayCheckOuts = allReservations.Count(r => 
            r.CheckOutDate == today && r.ReservationStatus == ReservationStatuses.CheckedIn);

        var totalActiveBookings = allReservations.Count(r => 
            r.ReservationStatus == ReservationStatuses.Booked);

        var totalCheckedIn = allReservations.Count(r => 
            r.ReservationStatus == ReservationStatuses.CheckedIn);

        // Receptionist receives operational dashboard only
        if (userRole == Roles.Receptionist)
        {
            return new DashboardResponseDto(
                TodayCheckIns: todayCheckIns,
                TodayCheckOuts: todayCheckOuts,
                TotalActiveBookings: totalActiveBookings,
                TotalCheckedIn: totalCheckedIn,
                TotalRevenue: null,
                OutstandingBalances: null,
                TotalReservations: null
            );
        }

        // Owner and Manager receive financial + operational overview
        var allPayments = (await _paymentRepository.GetPaymentsForReportAsync(null, null)).ToList();
        var totalRevenue = allPayments.Sum(p => p.Amount);

        var totalBookedAmount = allReservations
            .Where(r => r.ReservationStatus != ReservationStatuses.Cancelled)
            .Sum(r => r.TotalAmount);

        var outstanding = Math.Max(0, totalBookedAmount - totalRevenue);

        return new DashboardResponseDto(
            TodayCheckIns: todayCheckIns,
            TodayCheckOuts: todayCheckOuts,
            TotalActiveBookings: totalActiveBookings,
            TotalCheckedIn: totalCheckedIn,
            TotalRevenue: totalRevenue,
            OutstandingBalances: outstanding,
            TotalReservations: allReservations.Count
        );
    }
}
