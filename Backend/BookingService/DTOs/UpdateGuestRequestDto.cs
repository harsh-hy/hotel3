using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs;

public class UpdateGuestRequestDto
{
    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PhoneNumber is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(20, ErrorMessage = "PhoneNumber cannot exceed 20 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    public string? Address { get; set; }

    public UpdateGuestRequestDto() { }

    public UpdateGuestRequestDto(string fullName, string phoneNumber, string? email, string? address)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }
}
