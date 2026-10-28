using BookingService.DTOs;
using BookingService.Entities;
using BookingService.Repositories;
using HotelManagement.Shared.Constants;
using HotelManagement.Shared.Exceptions;

namespace BookingService.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> RecordMockPaymentAsync(MockPaymentRequestDto request);
    Task<PaymentResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<PaymentResponseDto>> GetByReservationIdAsync(int reservationId);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IReservationRepository _reservationRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IReservationRepository reservationRepository)
    {
        _paymentRepository = paymentRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<PaymentResponseDto> RecordMockPaymentAsync(MockPaymentRequestDto request)
    {
        if (request.Amount <= 0)
        {
            throw new BadRequestException("Payment amount must be greater than zero.");
        }

        if (!PaymentMethods.IsValid(request.PaymentMethod))
        {
            throw new BadRequestException($"Invalid payment method. Allowed methods are: {string.Join(", ", PaymentMethods.All)}.");
        }

        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, includeDetails: false);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {request.ReservationId} not found.");
        }

        if (reservation.ReservationStatus == ReservationStatuses.Cancelled)
        {
            throw new BadRequestException("Cannot record payments for a cancelled reservation.");
        }

        var currentTotalPaid = await _paymentRepository.GetTotalPaidForReservationAsync(request.ReservationId);
        var outstandingBalance = reservation.TotalAmount - currentTotalPaid;

        if (request.Amount > outstandingBalance)
        {
            throw new BadRequestException(
                $"Payment amount ({request.Amount:F2}) cannot exceed the current outstanding balance ({outstandingBalance:F2}).");
        }

        var paymentStatus = string.IsNullOrWhiteSpace(request.PaymentStatus) 
            ? PaymentStatuses.Paid 
            : request.PaymentStatus;

        if (!PaymentStatuses.IsValid(paymentStatus))
        {
            throw new BadRequestException($"Invalid payment status. Allowed statuses are: {string.Join(", ", PaymentStatuses.All)}.");
        }

        // Generate unique server-side transaction reference
        var transactionRef = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var payment = new Payment
        {
            ReservationId = request.ReservationId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = paymentStatus,
            TransactionReference = transactionRef,
            PaymentDate = DateTime.UtcNow
        };

        var created = await _paymentRepository.AddAsync(payment);
        return MapToDto(created);
    }

    public async Task<PaymentResponseDto> GetByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
        {
            throw new NotFoundException($"Payment with ID {id} not found.");
        }

        return MapToDto(payment);
    }

    public async Task<IEnumerable<PaymentResponseDto>> GetByReservationIdAsync(int reservationId)
    {
        var exists = await _reservationRepository.GetByIdAsync(reservationId, includeDetails: false);
        if (exists == null)
        {
            throw new NotFoundException($"Reservation with ID {reservationId} not found.");
        }

        var payments = await _paymentRepository.GetByReservationIdAsync(reservationId);
        return payments.Select(MapToDto);
    }

    private static PaymentResponseDto MapToDto(Payment payment) =>
        new(
            payment.PaymentId,
            payment.ReservationId,
            payment.Amount,
            payment.PaymentMethod,
            payment.PaymentStatus,
            payment.TransactionReference,
            payment.PaymentDate
        );
}
