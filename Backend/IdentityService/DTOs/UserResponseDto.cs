namespace IdentityService.DTOs;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserResponseDto() { }

    public UserResponseDto(int userId, string username, string fullName, string role, bool isActive, DateTime createdAt)
    {
        UserId = userId;
        Username = username;
        FullName = fullName;
        Role = role;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}
