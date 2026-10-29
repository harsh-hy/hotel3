using BookingService.DTOs;
using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Services;
using BookingService.Common.Constants;
using BookingService.Common.Exceptions;
using Moq;
using NUnit.Framework;

namespace BookingService.Tests;

[TestFixture]
public class PaymentServiceTests
{
    private Mock<IPaymentRepository> _paymentRepositoryMock = null!;
    private Mock<IReservationRepository> _reservationRepositoryMock = null!;
    private PaymentService _paymentService = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();

        _paymentService = new PaymentService(
            _paymentRepositoryMock.Object,
            _reservationRepositoryMock.Object);
    }

    [Test]
    public void RecordMockPaymentAsync_ZeroOrNegativeAmount_ThrowsBadRequestException()
    {
        // Arrange
        var request = new MockPaymentRequestDto(1, 0, PaymentMethods.Cash, null);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _paymentService.RecordMockPaymentAsync(request));
    }

    [Test]
    public void RecordMockPaymentAsync_InvalidMethod_ThrowsBadRequestException()
    {
        // Arrange
        var request = new MockPaymentRequestDto(1, 100, "BitCoin", null);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _paymentService.RecordMockPaymentAsync(request));
    }

    [Test]
    public void RecordMockPaymentAsync_CancelledReservation_ThrowsBadRequestException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 1,
            TotalAmount = 300.00m,
            ReservationStatus = ReservationStatuses.Cancelled
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(1, false)).ReturnsAsync(reservation);

        var request = new MockPaymentRequestDto(1, 100, PaymentMethods.UPI, null);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _paymentService.RecordMockPaymentAsync(request));
    }

    [Test]
    public void RecordMockPaymentAsync_ExceedsOutstandingBalance_ThrowsBadRequestException()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 1,
            TotalAmount = 300.00m,
            ReservationStatus = ReservationStatuses.Booked
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(1, false)).ReturnsAsync(reservation);
        _paymentRepositoryMock.Setup(p => p.GetTotalPaidForReservationAsync(1)).ReturnsAsync(200.00m); // Balance = 100

        var request = new MockPaymentRequestDto(1, 150.00m, PaymentMethods.Card, null); // 150 > 100

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _paymentService.RecordMockPaymentAsync(request));
    }

    [Test]
    public async Task RecordMockPaymentAsync_ValidPayment_RecordsPaymentWithGeneratedReference()
    {
        // Arrange
        var reservation = new Reservation
        {
            ReservationId = 1,
            TotalAmount = 300.00m,
            ReservationStatus = ReservationStatuses.Booked
        };
        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(1, false)).ReturnsAsync(reservation);
        _paymentRepositoryMock.Setup(p => p.GetTotalPaidForReservationAsync(1)).ReturnsAsync(100.00m); // Balance = 200

        _paymentRepositoryMock.Setup(p => p.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => { p.PaymentId = 77; return p; });

        var request = new MockPaymentRequestDto(1, 200.00m, PaymentMethods.UPI, null);

        // Act
        var result = await _paymentService.RecordMockPaymentAsync(request);

        // Assert
        Assert.That(result.PaymentId, Is.EqualTo(77));
        Assert.That(result.Amount, Is.EqualTo(200.00m));
        Assert.That(result.PaymentStatus, Is.EqualTo(PaymentStatuses.Paid));
        Assert.That(result.TransactionReference, Does.StartWith("TXN-"));

        _paymentRepositoryMock.Verify(p => p.AddAsync(It.Is<Payment>(pay => 
            pay.Amount == 200.00m && pay.TransactionReference.StartsWith("TXN-"))), Times.Once);
    }
}
