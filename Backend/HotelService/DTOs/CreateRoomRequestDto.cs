using System.ComponentModel.DataAnnotations;

namespace HotelService.DTOs;

public class CreateRoomRequestDto
{
    [Required(ErrorMessage = "RoomNumber is required.")]
    [StringLength(20, ErrorMessage = "RoomNumber cannot exceed 20 characters.")]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "RoomTypeId must be greater than 0.")]
    public int RoomTypeId { get; set; }

    [Range(0.01, 1000000.00, ErrorMessage = "PricePerNight must be greater than 0.")]
    public decimal PricePerNight { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public CreateRoomRequestDto() { }

    public CreateRoomRequestDto(string roomNumber, int roomTypeId, decimal pricePerNight, string status, string? description)
    {
        RoomNumber = roomNumber;
        RoomTypeId = roomTypeId;
        PricePerNight = pricePerNight;
        Status = status;
        Description = description;
    }
}
