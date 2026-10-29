using IdentityService.DTOs;
using IdentityService.Entities;
using IdentityService.Repositories;
using IdentityService.Services;
using IdentityService.Common.Constants;
using IdentityService.Common.Exceptions;
using Moq;
using NUnit.Framework;

namespace IdentityService.Tests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private UserService _userService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_ReturnsAllMappedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { UserId = 1, Username = "admin", FullName = "Admin", Role = Roles.Owner, IsActive = true },
            new() { UserId = 2, Username = "mgr", FullName = "Manager", Role = Roles.Manager, IsActive = true }
        };

        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = (await _userService.GetAllUsersAsync()).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Username, Is.EqualTo("admin"));
        Assert.That(result[1].Role, Is.EqualTo(Roles.Manager));
    }

    [Test]
    public void CreateUserAsync_DuplicateUsername_ThrowsConflictException()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync("existinguser"))
            .ReturnsAsync(true);

        var request = new CreateUserRequestDto("existinguser", "Password123!", "Existing User", Roles.Receptionist);

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _userService.CreateUserAsync(request));
    }

    [Test]
    public void CreateUserAsync_InvalidRole_ThrowsBadRequestException()
    {
        // Arrange
        var request = new CreateUserRequestDto("newuser", "Password123!", "New User", "InvalidRole");

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _userService.CreateUserAsync(request));
    }

    [Test]
    public async Task CreateUserAsync_ValidRequest_CreatesUserWithHashedPassword()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync("newuser"))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => { u.UserId = 10; return u; });

        var request = new CreateUserRequestDto("newuser", "SecurePass123!", "Jane Doe", Roles.Receptionist);

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.That(result.UserId, Is.EqualTo(10));
        Assert.That(result.Username, Is.EqualTo("newuser"));
        Assert.That(result.Role, Is.EqualTo(Roles.Receptionist));
        Assert.That(result.IsActive, Is.True);

        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => 
            u.Username == "newuser" && 
            BCrypt.Net.BCrypt.Verify("SecurePass123!", u.PasswordHash))), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_UpdatesFieldsSuccessfully()
    {
        // Arrange
        var existing = new User
        {
            UserId = 5,
            Username = "staff5",
            FullName = "Old Name",
            Role = Roles.Receptionist,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);

        var updateRequest = new UpdateUserRequestDto("New Name", Roles.Manager, false);

        // Act
        var result = await _userService.UpdateUserAsync(5, updateRequest);

        // Assert
        Assert.That(result.FullName, Is.EqualTo("New Name"));
        Assert.That(result.Role, Is.EqualTo(Roles.Manager));
        Assert.That(result.IsActive, Is.False);

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => 
            u.FullName == "New Name" && u.Role == Roles.Manager && !u.IsActive)), Times.Once);
    }
}
