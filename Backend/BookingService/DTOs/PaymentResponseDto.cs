namespace BookingService.DTOs;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }

    public PaymentResponseDto() { }

    public PaymentResponseDto(int paymentId, int reservationId, decimal amount, string paymentMethod, string paymentStatus, string transactionReference, DateTime paymentDate)
    {
        PaymentId = paymentId;
        ReservationId = reservationId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
        TransactionReference = transactionReference;
        PaymentDate = paymentDate;
    }
}
