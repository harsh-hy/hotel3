using HotelService.DTOs;
using HotelService.Entities;
using HotelService.Repositories;
using HotelService.Services;
using HotelManagement.Shared.Exceptions;
using Moq;
using NUnit.Framework;

namespace HotelService.Tests;

[TestFixture]
public class RoomTypeServiceTests
{
    private Mock<IRoomTypeRepository> _roomTypeRepositoryMock = null!;
    private RoomTypeService _roomTypeService = null!;

    [SetUp]
    public void SetUp()
    {
        _roomTypeRepositoryMock = new Mock<IRoomTypeRepository>();
        _roomTypeService = new RoomTypeService(_roomTypeRepositoryMock.Object);
    }

    [Test]
    public void CreateAsync_DuplicateName_ThrowsConflictException()
    {
        // Arrange
        _roomTypeRepositoryMock.Setup(r => r.ExistsByNameAsync("Suite")).ReturnsAsync(true);
        var request = new CreateRoomTypeRequestDto("Suite", 4, "Luxury suite");

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _roomTypeService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_ZeroOrNegativeCapacity_ThrowsBadRequestException()
    {
        // Arrange
        var request = new CreateRoomTypeRequestDto("Single", 0, "No capacity");

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _roomTypeService.CreateAsync(request));
    }

    [Test]
    public async Task CreateAsync_ValidType_CreatesAndReturnsDto()
    {
        // Arrange
        _roomTypeRepositoryMock.Setup(r => r.ExistsByNameAsync("Deluxe")).ReturnsAsync(false);
        _roomTypeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RoomType>()))
            .ReturnsAsync((RoomType rt) => { rt.RoomTypeId = 1; return rt; });

        var request = new CreateRoomTypeRequestDto("Deluxe", 2, "Spacious room");

        // Act
        var result = await _roomTypeService.CreateAsync(request);

        // Assert
        Assert.That(result.TypeName, Is.EqualTo("Deluxe"));
        Assert.That(result.Capacity, Is.EqualTo(2));
    }
}
