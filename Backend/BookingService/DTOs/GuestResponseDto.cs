namespace BookingService.DTOs;

public class GuestResponseDto
{
    public int GuestId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }

    public GuestResponseDto() { }

    public GuestResponseDto(int guestId, string fullName, string phoneNumber, string? email, string? address, DateTime createdAt)
    {
        GuestId = guestId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        CreatedAt = createdAt;
    }
}
