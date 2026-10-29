using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs;

public class MockPaymentRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ReservationId must be greater than 0.")]
    public int ReservationId { get; set; }

    [Range(0.01, 10000000.00, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "PaymentMethod is required.")]
    public string PaymentMethod { get; set; } = string.Empty;

    public string? PaymentStatus { get; set; }

    public MockPaymentRequestDto() { }

    public MockPaymentRequestDto(int reservationId, decimal amount, string paymentMethod, string? paymentStatus)
    {
        ReservationId = reservationId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
    }
}
