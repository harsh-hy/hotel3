namespace BookingService.Entities;

public class Guest
{
    public int GuestId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class Reservation
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    
    // Logical cross-service reference only to HotelDB.Rooms.RoomId (NO SQL FK)
    public int RoomId { get; set; }

    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public decimal PricePerNightSnapshot { get; set; }
    public decimal TotalAmount { get; set; }
    public string ReservationStatus { get; set; } = "Booked";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guest? Guest { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class Payment
{
    public int PaymentId { get; set; }
    public int ReservationId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string PaymentStatus { get; set; } = "Paid";
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public Reservation? Reservation { get; set; }
}
