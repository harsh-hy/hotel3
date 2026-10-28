namespace BookingService.DTOs;

// Guest DTOs
public record CreateGuestRequestDto(string FullName, string PhoneNumber, string? Email, string? Address);
public record UpdateGuestRequestDto(string FullName, string PhoneNumber, string? Email, string? Address);
public record GuestResponseDto(int GuestId, string FullName, string PhoneNumber, string? Email, string? Address, DateTime CreatedAt);

// Reservation DTOs
public record CreateReservationRequestDto(
    int GuestId, 
    int RoomId, 
    DateOnly CheckInDate, 
    DateOnly CheckOutDate, 
    int NumberOfAdults, 
    int NumberOfChildren
);

public record UpdateReservationRequestDto(
    DateOnly CheckInDate, 
    DateOnly CheckOutDate, 
    int NumberOfAdults, 
    int NumberOfChildren
);

public record ReservationResponseDto(
    int ReservationId, 
    int GuestId, 
    string GuestName, 
    string GuestPhone,
    int RoomId, 
    DateOnly CheckInDate, 
    DateOnly CheckOutDate, 
    int NumberOfAdults, 
    int NumberOfChildren, 
    decimal PricePerNightSnapshot, 
    decimal TotalAmount, 
    string ReservationStatus, 
    DateTime CreatedAt, 
    DateTime? UpdatedAt
);

// Payment DTOs
public record MockPaymentRequestDto(int ReservationId, decimal Amount, string PaymentMethod, string? PaymentStatus);

public record PaymentResponseDto(
    int PaymentId, 
    int ReservationId, 
    decimal Amount, 
    string PaymentMethod, 
    string PaymentStatus, 
    string TransactionReference, 
    DateTime PaymentDate
);

// Bill DTO
public record BillResponseDto(
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
    DateTime GeneratedAt
);

// Dashboard DTO
public record DashboardResponseDto(
    int TodayCheckIns,
    int TodayCheckOuts,
    int TotalActiveBookings,
    int TotalCheckedIn,
    // Financial metrics included for Owner and Manager roles
    decimal? TotalRevenue,
    decimal? OutstandingBalances,
    int? TotalReservations
);

// Reports DTOs
public record RevenueReportResponseDto(
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal TotalRevenue,
    int TotalTransactions,
    Dictionary<string, decimal> RevenueByPaymentMethod
);

public record ReservationReportResponseDto(
    DateOnly? StartDate,
    DateOnly? EndDate,
    int TotalReservations,
    int TotalNightsBooked,
    decimal TotalRevenueBooked,
    Dictionary<string, int> StatusBreakdown
);

// External Room Info DTO from Hotel Service
public record ExternalRoomDto(
    int RoomId, 
    string RoomNumber, 
    int RoomTypeId, 
    string TypeName, 
    int Capacity, 
    decimal PricePerNight, 
    string Status
);
