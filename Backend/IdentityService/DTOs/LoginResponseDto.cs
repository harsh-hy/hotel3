namespace IdentityService.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    public LoginResponseDto() { }

    public LoginResponseDto(string token, string username, string role, string fullName, DateTime expiresAt)
    {
        Token = token;
        Username = username;
        Role = role;
        FullName = fullName;
        ExpiresAt = expiresAt;
    }
}
