namespace HotelService.DTOs;

public class RoomResponseDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }

    public RoomResponseDto() { }

    public RoomResponseDto(int roomId, string roomNumber, int roomTypeId, string typeName, int capacity, decimal pricePerNight, string status, string? description)
    {
        RoomId = roomId;
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        TypeName = typeName;
        Capacity = capacity;
        PricePerNight = pricePerNight;
        Status = status;
        Description = description;
    }
}
