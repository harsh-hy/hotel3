namespace HotelService.Entities;

public class RoomType
{
    public int RoomTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Description { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}

public class Room
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public decimal PricePerNight { get; set; }
    public string Status { get; set; } = "Available";
    public string? Description { get; set; }

    public RoomType? RoomType { get; set; }
}
