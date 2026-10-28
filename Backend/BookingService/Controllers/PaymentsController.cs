using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Authorize(Roles = "Owner,Manager,Receptionist")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("api/booking/payments/mock")]
    public async Task<IActionResult> RecordMockPayment([FromBody] MockPaymentRequestDto request)
    {
        var payment = await _paymentService.RecordMockPaymentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, payment);
    }

    [HttpGet("api/booking/payments/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        return Ok(payment);
    }

    [HttpGet("api/booking/reservations/{reservationId:int}/payments")]
    public async Task<IActionResult> GetByReservationId(int reservationId)
    {
        var payments = await _paymentService.GetByReservationIdAsync(reservationId);
        return Ok(payments);
    }
}
