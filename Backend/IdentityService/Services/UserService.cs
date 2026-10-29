using IdentityService.DTOs;
using IdentityService.Entities;
using IdentityService.Repositories;
using IdentityService.Common.Constants;
using IdentityService.Common.Exceptions;

namespace IdentityService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        return MapToDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new BadRequestException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            throw new BadRequestException("Password must be at least 6 characters long.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Full name is required.");
        }

        if (!Roles.IsValid(request.Role) || request.Role == Roles.Owner)
        {
            throw new BadRequestException($"Invalid staff role '{request.Role}'. Allowed roles are: {Roles.Manager}, {Roles.Receptionist}.");
        }

        var normalizedUsername = request.Username.Trim();
        if (await _userRepository.ExistsByUsernameAsync(normalizedUsername))
        {
            throw new ConflictException($"Username '{normalizedUsername}' is already taken.");
        }

        var user = new User
        {
            Username = normalizedUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName.Trim(),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.AddAsync(user);
        return MapToDto(created);
    }

    public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Full name is required.");
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        if (user.Role == Roles.Owner)
        {
            if (request.Role != Roles.Owner)
            {
                throw new BadRequestException("The Owner account role cannot be altered.");
            }
            if (!request.IsActive)
            {
                throw new BadRequestException("The primary Owner account cannot be deactivated.");
            }
        }
        else
        {
            if (request.Role == Roles.Owner)
            {
                throw new BadRequestException("Staff accounts cannot be elevated to Owner.");
            }
            if (!Roles.IsValid(request.Role))
            {
                throw new BadRequestException($"Invalid staff role '{request.Role}'. Allowed roles are: {Roles.Manager}, {Roles.Receptionist}.");
            }
        }

        user.FullName = request.FullName.Trim();
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    private static UserResponseDto MapToDto(User user) =>
        new(user.UserId, user.Username, user.FullName, user.Role, user.IsActive, user.CreatedAt);
}
