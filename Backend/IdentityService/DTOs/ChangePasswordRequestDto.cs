using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
    public string NewPassword { get; set; } = string.Empty;

    public ChangePasswordRequestDto() { }

    public ChangePasswordRequestDto(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
