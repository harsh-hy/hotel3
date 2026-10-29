namespace BookingService.DTOs;

public class ExternalRoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string Status { get; set; } = string.Empty;

    public ExternalRoomDto() { }

    public ExternalRoomDto(int roomId, string roomNumber, int roomTypeId, string typeName, int capacity, decimal pricePerNight, string status)
    {
        RoomId = roomId;
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        TypeName = typeName;
        Capacity = capacity;
        PricePerNight = pricePerNight;
        Status = status;
    }
}
