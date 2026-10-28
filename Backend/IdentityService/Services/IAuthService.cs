using IdentityService.DTOs;

namespace IdentityService.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<UserResponseDto> GetCurrentUserAsync(int userId);
}
