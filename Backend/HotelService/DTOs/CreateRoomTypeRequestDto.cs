using System.ComponentModel.DataAnnotations;

namespace HotelService.DTOs;

public class CreateRoomTypeRequestDto
{
    [Required(ErrorMessage = "TypeName is required.")]
    [StringLength(50, ErrorMessage = "TypeName cannot exceed 50 characters.")]
    public string TypeName { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20.")]
    public int Capacity { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public CreateRoomTypeRequestDto() { }

    public CreateRoomTypeRequestDto(string typeName, int capacity, string? description)
    {
        TypeName = typeName;
        Capacity = capacity;
        Description = description;
    }
}
