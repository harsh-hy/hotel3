using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs;

public class UpdateReservationRequestDto
{
    [Required(ErrorMessage = "CheckInDate is required.")]
    public DateOnly CheckInDate { get; set; }

    [Required(ErrorMessage = "CheckOutDate is required.")]
    public DateOnly CheckOutDate { get; set; }

    [Range(1, 10, ErrorMessage = "NumberOfAdults must be between 1 and 10.")]
    public int NumberOfAdults { get; set; }

    [Range(0, 10, ErrorMessage = "NumberOfChildren must be between 0 and 10.")]
    public int NumberOfChildren { get; set; }

    public UpdateReservationRequestDto() { }

    public UpdateReservationRequestDto(DateOnly checkInDate, DateOnly checkOutDate, int numberOfAdults, int numberOfChildren)
    {
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        NumberOfAdults = numberOfAdults;
        NumberOfChildren = numberOfChildren;
    }
}
