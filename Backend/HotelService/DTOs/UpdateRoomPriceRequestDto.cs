using System.ComponentModel.DataAnnotations;

namespace HotelService.DTOs;

public class UpdateRoomPriceRequestDto
{
    [Range(0.01, 1000000.00, ErrorMessage = "PricePerNight must be greater than 0.")]
    public decimal PricePerNight { get; set; }

    public UpdateRoomPriceRequestDto() { }

    public UpdateRoomPriceRequestDto(decimal pricePerNight)
    {
        PricePerNight = pricePerNight;
    }
}
