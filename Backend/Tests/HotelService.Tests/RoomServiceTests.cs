using HotelService.DTOs;
using HotelService.Entities;
using HotelService.Repositories;
using HotelService.Services;
using HotelService.Services.Http;
using HotelService.Common.Constants;
using HotelService.Common.Exceptions;
using Moq;
using NUnit.Framework;

namespace HotelService.Tests;

[TestFixture]
public class RoomServiceTests
{
    private Mock<IRoomRepository> _roomRepositoryMock = null!;
    private Mock<IRoomTypeRepository> _roomTypeRepositoryMock = null!;
    private Mock<IBookingServiceClient> _bookingServiceClientMock = null!;
    private RoomService _roomService = null!;

    [SetUp]
    public void SetUp()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _roomTypeRepositoryMock = new Mock<IRoomTypeRepository>();
        _bookingServiceClientMock = new Mock<IBookingServiceClient>();

        _roomService = new RoomService(
            _roomRepositoryMock.Object,
            _roomTypeRepositoryMock.Object,
            _bookingServiceClientMock.Object);
    }

    [Test]
    public void CreateAsync_DuplicateRoomNumber_ThrowsConflictException()
    {
        // Arrange
        _roomTypeRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new RoomType { RoomTypeId = 1, TypeName = "Deluxe", Capacity = 2 });

        _roomRepositoryMock.Setup(r => r.ExistsByRoomNumberAsync("101"))
            .ReturnsAsync(true);

        var request = new CreateRoomRequestDto("101", 1, 150.00m, RoomStatuses.Available, "Sea view");

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _roomService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_NegativePrice_ThrowsBadRequestException()
    {
        // Arrange
        var request = new CreateRoomRequestDto("102", 1, -50.00m, RoomStatuses.Available, "Sea view");

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _roomService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_NonExistentRoomType_ThrowsNotFoundException()
    {
        // Arrange
        _roomTypeRepositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((RoomType?)null);

        var request = new CreateRoomRequestDto("102", 99, 100.00m, RoomStatuses.Available, null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _roomService.CreateAsync(request));
    }

    [Test]
    public async Task CreateAsync_ValidRoom_CreatesAndReturnsDto()
    {
        // Arrange
        var roomType = new RoomType { RoomTypeId = 1, TypeName = "Standard", Capacity = 2 };
        _roomTypeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(roomType);
        _roomRepositoryMock.Setup(r => r.ExistsByRoomNumberAsync("201")).ReturnsAsync(false);
        _roomRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Room>()))
            .ReturnsAsync((Room r) => { r.RoomId = 1; return r; });

        var request = new CreateRoomRequestDto("201", 1, 120.00m, RoomStatuses.Available, "Cozy");

        // Act
        var result = await _roomService.CreateAsync(request);

        // Assert
        Assert.That(result.RoomNumber, Is.EqualTo("201"));
        Assert.That(result.PricePerNight, Is.EqualTo(120.00m));
        Assert.That(result.TypeName, Is.EqualTo("Standard"));
    }

    [Test]
    public void DeleteAsync_WhenRoomIsOccupied_ThrowsConflictException()
    {
        // Arrange
        var occupiedRoom = new Room { RoomId = 5, RoomNumber = "105", Status = RoomStatuses.Occupied };
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(occupiedRoom);

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _roomService.DeleteAsync(5));
    }

    [Test]
    public void DeleteAsync_WhenRoomHasActiveReservations_ThrowsConflictException()
    {
        // Arrange
        var room = new Room { RoomId = 6, RoomNumber = "106", Status = RoomStatuses.Available };
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(room);
        _bookingServiceClientMock.Setup(b => b.HasActiveReservationsForRoomAsync(6)).ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _roomService.DeleteAsync(6));
    }

    [Test]
    public async Task DeleteAsync_WhenRoomIsSafeToDelete_DeletesSuccessfully()
    {
        // Arrange
        var room = new Room { RoomId = 7, RoomNumber = "107", Status = RoomStatuses.Available };
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(room);
        _bookingServiceClientMock.Setup(b => b.HasActiveReservationsForRoomAsync(7)).ReturnsAsync(false);

        // Act
        await _roomService.DeleteAsync(7);

        // Assert
        _roomRepositoryMock.Verify(r => r.DeleteAsync(room), Times.Once);
    }
}
