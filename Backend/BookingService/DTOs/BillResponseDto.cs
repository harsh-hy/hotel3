namespace BookingService.DTOs;

public class BillResponseDto
{
    public string BillReference { get; set; } = string.Empty;
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal TotalRoomCharges { get; set; }
    public decimal PaymentsReceived { get; set; }
    public decimal OutstandingBalance { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public BillResponseDto() { }

    public BillResponseDto(
        string BillReference,
        int ReservationId,
        int GuestId,
        string GuestName,
        string PhoneNumber,
        string? Email,
        int RoomId,
        DateOnly CheckInDate,
        DateOnly CheckOutDate,
        int NumberOfNights,
        decimal PricePerNight,
        decimal TotalRoomCharges,
        decimal PaymentsReceived,
        decimal OutstandingBalance,
        string PaymentStatus,
        DateTime GeneratedAt)
    {
        this.BillReference = BillReference;
        this.ReservationId = ReservationId;
        this.GuestId = GuestId;
        this.GuestName = GuestName;
        this.PhoneNumber = PhoneNumber;
        this.Email = Email;
        this.RoomId = RoomId;
        this.CheckInDate = CheckInDate;
        this.CheckOutDate = CheckOutDate;
        this.NumberOfNights = NumberOfNights;
        this.PricePerNight = PricePerNight;
        this.TotalRoomCharges = TotalRoomCharges;
        this.PaymentsReceived = PaymentsReceived;
        this.OutstandingBalance = OutstandingBalance;
        this.PaymentStatus = PaymentStatus;
        this.GeneratedAt = GeneratedAt;
    }
}
