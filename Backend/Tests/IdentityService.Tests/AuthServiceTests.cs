using IdentityService.DTOs;
using IdentityService.Entities;
using IdentityService.Repositories;
using IdentityService.Services;
using IdentityService.Common.Constants;
using IdentityService.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace IdentityService.Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private IConfiguration _configuration = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:SecretKey", "HotelManagementSystemSecretKey2026!AuthoritativeJwtSigningKey#SecureHotel123456"},
            {"Jwt:Issuer", "HotelManagement.IdentityService"},
            {"Jwt:Audience", "HotelManagement.Clients"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(_userRepositoryMock.Object, _configuration);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponseWithToken()
    {
        // Arrange
        var password = "ValidPassword123!";
        var user = new User
        {
            UserId = 1,
            Username = "staff1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = "John Doe",
            Role = Roles.Receptionist,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("staff1"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto("staff1", password);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo("staff1"));
        Assert.That(result.Role, Is.EqualTo(Roles.Receptionist));
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void LoginAsync_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "staff1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("staff1"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto("staff1", "WrongPassword");

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
    }

    [Test]
    public void LoginAsync_InactiveUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var password = "ValidPassword123!";
        var user = new User
        {
            UserId = 1,
            Username = "inactive_user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = false
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync("inactive_user"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto("inactive_user", password);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
    }

    [Test]
    public void ChangePasswordAsync_IncorrectCurrentPassword_ThrowsBadRequestException()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Username = "staff1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ExistingPassword"),
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        var request = new ChangePasswordRequestDto("WrongCurrentPassword", "NewPassword123!");

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _authService.ChangePasswordAsync(1, request));
    }

    [Test]
    public async Task ChangePasswordAsync_ValidCurrentPassword_UpdatesPasswordHash()
    {
        // Arrange
        var oldPassword = "OldPassword123!";
        var user = new User
        {
            UserId = 1,
            Username = "staff1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(oldPassword),
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        var request = new ChangePasswordRequestDto(oldPassword, "NewPassword123!");

        // Act
        await _authService.ChangePasswordAsync(1, request);

        // Assert
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => 
            BCrypt.Net.BCrypt.Verify("NewPassword123!", u.PasswordHash))), Times.Once);
    }
}
