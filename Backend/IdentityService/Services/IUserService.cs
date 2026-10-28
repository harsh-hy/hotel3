using IdentityService.DTOs;

namespace IdentityService.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserRequestDto request);
}
