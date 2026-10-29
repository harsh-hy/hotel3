using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs;

public class CreateUserRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; } = string.Empty;

    public CreateUserRequestDto() { }

    public CreateUserRequestDto(string username, string password, string fullName, string role)
    {
        Username = username;
        Password = password;
        FullName = fullName;
        Role = role;
    }
}
