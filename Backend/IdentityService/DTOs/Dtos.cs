namespace IdentityService.DTOs;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string Token, string Username, string Role, string FullName, DateTime ExpiresAt);

public record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);

public record CreateUserRequestDto(string Username, string Password, string FullName, string Role);

public record UpdateUserRequestDto(string FullName, string Role, bool IsActive);

public record UserResponseDto(int UserId, string Username, string FullName, string Role, bool IsActive, DateTime CreatedAt);
