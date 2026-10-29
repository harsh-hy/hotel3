namespace HotelService.DTOs;

public class RoomAvailabilityResponseDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string? Reason { get; set; }

    public RoomAvailabilityResponseDto() { }

    public RoomAvailabilityResponseDto(int roomId, string roomNumber, int roomTypeId, string typeName, int capacity, decimal pricePerNight, string status, bool isAvailable, string? reason)
    {
        RoomId = roomId;
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        TypeName = typeName;
        Capacity = capacity;
        PricePerNight = pricePerNight;
        Status = status;
        IsAvailable = isAvailable;
        Reason = reason;
    }
}
