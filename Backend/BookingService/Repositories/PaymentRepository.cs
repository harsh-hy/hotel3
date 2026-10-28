using BookingService.Data;
using BookingService.Entities;
using HotelManagement.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BookingDbContext _context;

    public PaymentRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.Reservation)
            .FirstOrDefaultAsync(p => p.PaymentId == id);
    }

    public async Task<IEnumerable<Payment>> GetByReservationIdAsync(int reservationId)
    {
        return await _context.Payments
            .Where(p => p.ReservationId == reservationId)
            .OrderByDescending(p => p.PaymentDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Payment> AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<bool> ExistsByTransactionReferenceAsync(string reference)
    {
        return await _context.Payments.AnyAsync(p => p.TransactionReference == reference);
    }

    public async Task<decimal> GetTotalPaidForReservationAsync(int reservationId)
    {
        return await _context.Payments
            .Where(p => p.ReservationId == reservationId && p.PaymentStatus == PaymentStatuses.Paid)
            .SumAsync(p => p.Amount);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsForReportAsync(DateOnly? startDate, DateOnly? endDate)
    {
        var query = _context.Payments
            .Where(p => p.PaymentStatus == PaymentStatuses.Paid)
            .AsQueryable();

        if (startDate.HasValue)
        {
            var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(p => p.PaymentDate >= startDateTime);
        }

        if (endDate.HasValue)
        {
            var endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(p => p.PaymentDate <= endDateTime);
        }

        return await query.AsNoTracking().ToListAsync();
    }
}
