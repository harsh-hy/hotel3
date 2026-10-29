namespace HotelService.DTOs;

public class RoomTypeResponseDto
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Description { get; set; }

    public RoomTypeResponseDto() { }

    public RoomTypeResponseDto(int roomTypeId, string typeName, int capacity, string? description)
    {
        RoomTypeId = roomTypeId;
        TypeName = typeName;
        Capacity = capacity;
        Description = description;
    }
}
