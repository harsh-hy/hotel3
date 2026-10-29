using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Services;
using BookingService.Common.Exceptions;
using Moq;
using NUnit.Framework;

namespace BookingService.Tests;

[TestFixture]
public class BillServiceTests
{
    private Mock<IReservationRepository> _reservationRepositoryMock = null!;
    private Mock<IPaymentRepository> _paymentRepositoryMock = null!;
    private BillService _billService = null!;

    [SetUp]
    public void SetUp()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();

        _billService = new BillService(
            _reservationRepositoryMock.Object,
            _paymentRepositoryMock.Object);
    }

    [Test]
    public void GetBillForReservationAsync_NonExistentReservation_ThrowsNotFoundException()
    {
        // Arrange
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(999, true))
            .ReturnsAsync((Reservation?)null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _billService.GetBillForReservationAsync(999));
    }

    [Test]
    public async Task GetBillForReservationAsync_ValidReservation_CalculatesChargesAndBalance()
    {
        // Arrange
        var checkIn = new DateOnly(2026, 11, 1);
        var checkOut = new DateOnly(2026, 11, 4); // 3 nights

        var reservation = new Reservation
        {
            ReservationId = 15,
            GuestId = 2,
            RoomId = 101,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            PricePerNightSnapshot = 120.00m,
            TotalAmount = 360.00m,
            Guest = new Guest { GuestId = 2, FullName = "Sarah Connor", PhoneNumber = "555-1234" }
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(15, true)).ReturnsAsync(reservation);
        _paymentRepositoryMock.Setup(p => p.GetTotalPaidForReservationAsync(15)).ReturnsAsync(200.00m);

        // Act
        var bill = await _billService.GetBillForReservationAsync(15);

        // Assert
        Assert.That(bill.BillReference, Is.EqualTo("BILL-000015"));
        Assert.That(bill.GuestName, Is.EqualTo("Sarah Connor"));
        Assert.That(bill.NumberOfNights, Is.EqualTo(3));
        Assert.That(bill.PricePerNight, Is.EqualTo(120.00m));
        Assert.That(bill.TotalRoomCharges, Is.EqualTo(360.00m));
        Assert.That(bill.PaymentsReceived, Is.EqualTo(200.00m));
        Assert.That(bill.OutstandingBalance, Is.EqualTo(160.00m));
        Assert.That(bill.PaymentStatus, Is.EqualTo("PartiallyPaid"));
    }

    [Test]
    public async Task GetBillForReservationAsync_FullyPaid_MarksStatusAsPaid()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 16,
            CheckInDate = new DateOnly(2026, 11, 1),
            CheckOutDate = new DateOnly(2026, 11, 2),
            PricePerNightSnapshot = 100.00m,
            TotalAmount = 100.00m,
            Guest = new Guest { GuestId = 1, FullName = "Alice" }
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(16, true)).ReturnsAsync(reservation);
        _paymentRepositoryMock.Setup(p => p.GetTotalPaidForReservationAsync(16)).ReturnsAsync(100.00m);

        // Act
        var bill = await _billService.GetBillForReservationAsync(16);

        // Assert
        Assert.That(bill.OutstandingBalance, Is.EqualTo(0.00m));
        Assert.That(bill.PaymentStatus, Is.EqualTo("Paid"));
    }
}
