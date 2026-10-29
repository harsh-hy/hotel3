using BookingService.DTOs;
using BookingService.Repositories;
using BookingService.Common.Exceptions;

namespace BookingService.Services;

public interface IBillService
{
    Task<BillResponseDto> GetBillForReservationAsync(int reservationId);
}

public class BillService : IBillService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;

    public BillService(
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<BillResponseDto> GetBillForReservationAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {reservationId} not found.");
        }

        var nights = (reservation.CheckOutDate.ToDateTime(TimeOnly.MinValue) - 
                      reservation.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
        
        var totalPaid = await _paymentRepository.GetTotalPaidForReservationAsync(reservationId);
        var balance = Math.Max(0, reservation.TotalAmount - totalPaid);

        string paymentStatus = totalPaid >= reservation.TotalAmount 
            ? "Paid" 
            : (totalPaid > 0 ? "PartiallyPaid" : "Unpaid");

        return new BillResponseDto(
            BillReference: $"BILL-{reservation.ReservationId:D6}",
            ReservationId: reservation.ReservationId,
            GuestId: reservation.GuestId,
            GuestName: reservation.Guest?.FullName ?? "Unknown Guest",
            PhoneNumber: reservation.Guest?.PhoneNumber ?? "N/A",
            Email: reservation.Guest?.Email,
            RoomId: reservation.RoomId,
            CheckInDate: reservation.CheckInDate,
            CheckOutDate: reservation.CheckOutDate,
            NumberOfNights: nights,
            PricePerNight: reservation.PricePerNightSnapshot,
            TotalRoomCharges: reservation.TotalAmount,
            PaymentsReceived: totalPaid,
            OutstandingBalance: balance,
            PaymentStatus: paymentStatus,
            GeneratedAt: DateTime.UtcNow
        );
    }
}
