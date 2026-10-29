using BookingService.DTOs;
using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Services;
using BookingService.Services.Http;
using BookingService.Common.Constants;
using BookingService.Common.Exceptions;
using Moq;
using NUnit.Framework;

namespace BookingService.Tests;

[TestFixture]
public class ReservationServiceTests
{
    private Mock<IReservationRepository> _reservationRepositoryMock = null!;
    private Mock<IGuestRepository> _guestRepositoryMock = null!;
    private Mock<IHotelServiceClient> _hotelServiceClientMock = null!;
    private ReservationService _reservationService = null!;

    [SetUp]
    public void SetUp()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _guestRepositoryMock = new Mock<IGuestRepository>();
        _hotelServiceClientMock = new Mock<IHotelServiceClient>();

        _reservationService = new ReservationService(
            _reservationRepositoryMock.Object,
            _guestRepositoryMock.Object,
            _hotelServiceClientMock.Object);
    }

    [Test]
    public void CreateAsync_CheckInInPast_ThrowsBadRequestException()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var request = new CreateReservationRequestDto(1, 101, pastDate, checkOut, 2, 0);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_CheckOutBeforeOrOnCheckIn_ThrowsBadRequestException()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var checkOut = checkIn; // Same day
        var request = new CreateReservationRequestDto(1, 101, checkIn, checkOut, 2, 0);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_GuestNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        _guestRepositoryMock.Setup(g => g.GetByIdAsync(99)).ReturnsAsync((Guest?)null);

        var request = new CreateReservationRequestDto(99, 101, checkIn, checkOut, 2, 0);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_RoomNotAvailableInHotelService_ThrowsBadRequestException()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

        _guestRepositoryMock.Setup(g => g.GetByIdAsync(1))
            .ReturnsAsync(new Guest { GuestId = 1, FullName = "John" });

        var maintenanceRoom = new ExternalRoomDto(101, "101", 1, "Deluxe", 2, 100.00m, RoomStatuses.Maintenance);
        _hotelServiceClientMock.Setup(h => h.GetRoomDetailsAsync(101)).ReturnsAsync(maintenanceRoom);

        var request = new CreateReservationRequestDto(1, 101, checkIn, checkOut, 2, 0);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_PartyExceedsCapacity_ThrowsBadRequestException()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

        _guestRepositoryMock.Setup(g => g.GetByIdAsync(1))
            .ReturnsAsync(new Guest { GuestId = 1, FullName = "John" });

        var smallRoom = new ExternalRoomDto(101, "101", 1, "Single", 1, 80.00m, RoomStatuses.Available);
        _hotelServiceClientMock.Setup(h => h.GetRoomDetailsAsync(101)).ReturnsAsync(smallRoom);

        var request = new CreateReservationRequestDto(1, 101, checkIn, checkOut, 2, 1); // 3 guests > 1 capacity

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public void CreateAsync_ConflictingBookingExists_ThrowsConflictException()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(8));

        _guestRepositoryMock.Setup(g => g.GetByIdAsync(1))
            .ReturnsAsync(new Guest { GuestId = 1, FullName = "John" });

        var room = new ExternalRoomDto(101, "101", 1, "Deluxe", 2, 150.00m, RoomStatuses.Available);
        _hotelServiceClientMock.Setup(h => h.GetRoomDetailsAsync(101)).ReturnsAsync(room);

        var existingConflict = new List<Reservation>
        {
            new() { ReservationId = 10, RoomId = 101, ReservationStatus = ReservationStatuses.Booked }
        };
        _reservationRepositoryMock.Setup(r => r.GetConflictingReservationsAsync(101, checkIn, checkOut, null))
            .ReturnsAsync(existingConflict);

        var request = new CreateReservationRequestDto(1, 101, checkIn, checkOut, 2, 0);

        // Act & Assert
        Assert.ThrowsAsync<ConflictException>(() => _reservationService.CreateAsync(request));
    }

    [Test]
    public async Task CreateAsync_ValidBooking_CalculatesTotalAndStoresSnapshotPrice()
    {
        // Arrange
        var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
        var checkOut = DateOnly.FromDateTime(DateTime.Today.AddDays(5)); // 3 nights
        var nightlyPrice = 150.00m;

        _guestRepositoryMock.Setup(g => g.GetByIdAsync(1))
            .ReturnsAsync(new Guest { GuestId = 1, FullName = "Alice Smith", PhoneNumber = "1234567890" });

        var room = new ExternalRoomDto(105, "105", 2, "Deluxe", 3, nightlyPrice, RoomStatuses.Available);
        _hotelServiceClientMock.Setup(h => h.GetRoomDetailsAsync(105)).ReturnsAsync(room);

        _reservationRepositoryMock.Setup(r => r.GetConflictingReservationsAsync(105, checkIn, checkOut, null))
            .ReturnsAsync(new List<Reservation>());

        _reservationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation res) => { res.ReservationId = 42; return res; });

        var request = new CreateReservationRequestDto(1, 105, checkIn, checkOut, 2, 0);

        // Act
        var result = await _reservationService.CreateAsync(request);

        // Assert
        Assert.That(result.ReservationId, Is.EqualTo(42));
        Assert.That(result.PricePerNightSnapshot, Is.EqualTo(150.00m));
        Assert.That(result.TotalAmount, Is.EqualTo(450.00m)); // 3 nights * 150
        Assert.That(result.ReservationStatus, Is.EqualTo(ReservationStatuses.Booked));
    }

    [Test]
    public async Task StatusTransitions_BookedToCheckedIn_Succeeds()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 1,
            ReservationStatus = ReservationStatuses.Booked
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(1, true)).ReturnsAsync(reservation);

        // Act
        var result = await _reservationService.CheckInAsync(1);

        // Assert
        Assert.That(result.ReservationStatus, Is.EqualTo(ReservationStatuses.CheckedIn));
        _reservationRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Reservation>(res => 
            res.ReservationStatus == ReservationStatuses.CheckedIn)), Times.Once);
    }

    [Test]
    public async Task StatusTransitions_CheckedInToCheckedOut_Succeeds()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 2,
            ReservationStatus = ReservationStatuses.CheckedIn
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(2, true)).ReturnsAsync(reservation);

        // Act
        var result = await _reservationService.CheckOutAsync(2);

        // Assert
        Assert.That(result.ReservationStatus, Is.EqualTo(ReservationStatuses.CheckedOut));
        _reservationRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Reservation>(res => 
            res.ReservationStatus == ReservationStatuses.CheckedOut)), Times.Once);
    }

    [Test]
    public void StatusTransitions_CancelledToCheckedIn_ThrowsBadRequestException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 3,
            ReservationStatus = ReservationStatuses.Cancelled
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(3, true)).ReturnsAsync(reservation);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _reservationService.CheckInAsync(3));
    }
}
