using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs;

public class UpdateUserRequestDto
{
    [Required(ErrorMessage = "FullName is required.")]
    [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public UpdateUserRequestDto() { }

    public UpdateUserRequestDto(string fullName, string role, bool isActive)
    {
        FullName = fullName;
        Role = role;
        IsActive = isActive;
    }
}
