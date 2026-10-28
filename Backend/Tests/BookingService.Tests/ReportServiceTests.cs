using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Services;
using HotelManagement.Shared.Constants;
using Moq;
using NUnit.Framework;

namespace BookingService.Tests;

[TestFixture]
public class ReportServiceTests
{
    private Mock<IPaymentRepository> _paymentRepositoryMock = null!;
    private Mock<IReservationRepository> _reservationRepositoryMock = null!;
    private ReportService _reportService = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();

        _reportService = new ReportService(
            _paymentRepositoryMock.Object,
            _reservationRepositoryMock.Object);
    }

    [Test]
    public async Task GetRevenueReportAsync_AggregatesRevenueByMethod()
    {
        // Arrange
        var payments = new List<Payment>
        {
            new() { Amount = 100.00m, PaymentMethod = PaymentMethods.Cash, PaymentStatus = PaymentStatuses.Paid },
            new() { Amount = 150.00m, PaymentMethod = PaymentMethods.Card, PaymentStatus = PaymentStatuses.Paid },
            new() { Amount = 200.00m, PaymentMethod = PaymentMethods.UPI, PaymentStatus = PaymentStatuses.Paid },
            new() { Amount = 50.00m, PaymentMethod = PaymentMethods.UPI, PaymentStatus = PaymentStatuses.Paid }
        };

        _paymentRepositoryMock.Setup(p => p.GetPaymentsForReportAsync(null, null))
            .ReturnsAsync(payments);

        // Act
        var result = await _reportService.GetRevenueReportAsync(null, null);

        // Assert
        Assert.That(result.TotalRevenue, Is.EqualTo(500.00m));
        Assert.That(result.TotalTransactions, Is.EqualTo(4));
        Assert.That(result.RevenueByPaymentMethod[PaymentMethods.Cash], Is.EqualTo(100.00m));
        Assert.That(result.RevenueByPaymentMethod[PaymentMethods.Card], Is.EqualTo(150.00m));
        Assert.That(result.RevenueByPaymentMethod[PaymentMethods.UPI], Is.EqualTo(250.00m));
    }

    [Test]
    public async Task GetReservationReportAsync_AggregatesCountsAndStatuses()
    {
        // Arrange
        var checkIn1 = new DateOnly(2026, 11, 1);
        var checkOut1 = new DateOnly(2026, 11, 3); // 2 nights

        var checkIn2 = new DateOnly(2026, 11, 5);
        var checkOut2 = new DateOnly(2026, 11, 8); // 3 nights

        var reservations = new List<Reservation>
        {
            new() { CheckInDate = checkIn1, CheckOutDate = checkOut1, TotalAmount = 200.00m, ReservationStatus = ReservationStatuses.Booked },
            new() { CheckInDate = checkIn2, CheckOutDate = checkOut2, TotalAmount = 300.00m, ReservationStatus = ReservationStatuses.CheckedIn },
            new() { CheckInDate = checkIn1, CheckOutDate = checkOut1, TotalAmount = 150.00m, ReservationStatus = ReservationStatuses.Cancelled }
        };

        _reservationRepositoryMock.Setup(r => r.GetReservationsForReportAsync(null, null))
            .ReturnsAsync(reservations);

        // Act
        var result = await _reportService.GetReservationReportAsync(null, null);

        // Assert
        Assert.That(result.TotalReservations, Is.EqualTo(3));
        Assert.That(result.TotalNightsBooked, Is.EqualTo(7)); // 2 + 3 + 2
        Assert.That(result.TotalRevenueBooked, Is.EqualTo(500.00m)); // Excludes cancelled 150
        Assert.That(result.StatusBreakdown[ReservationStatuses.Booked], Is.EqualTo(1));
        Assert.That(result.StatusBreakdown[ReservationStatuses.CheckedIn], Is.EqualTo(1));
        Assert.That(result.StatusBreakdown[ReservationStatuses.Cancelled], Is.EqualTo(1));
    }
}
