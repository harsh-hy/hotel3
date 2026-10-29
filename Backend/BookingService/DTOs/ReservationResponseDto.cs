namespace BookingService.DTOs;

public class ReservationResponseDto
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestPhone { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public decimal PricePerNightSnapshot { get; set; }
    public decimal TotalAmount { get; set; }
    public string ReservationStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ReservationResponseDto() { }

    public ReservationResponseDto(
        int reservationId,
        int guestId,
        string guestName,
        string guestPhone,
        int roomId,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int numberOfAdults,
        int numberOfChildren,
        decimal pricePerNightSnapshot,
        decimal totalAmount,
        string reservationStatus,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        ReservationId = reservationId;
        GuestId = guestId;
        GuestName = guestName;
        GuestPhone = guestPhone;
        RoomId = roomId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        NumberOfAdults = numberOfAdults;
        NumberOfChildren = numberOfChildren;
        PricePerNightSnapshot = pricePerNightSnapshot;
        TotalAmount = totalAmount;
        ReservationStatus = reservationStatus;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
